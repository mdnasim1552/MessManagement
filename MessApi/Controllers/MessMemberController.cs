using MessApi.Models;
using MessApi.UnitOfWork;
using MessManagement.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessMemberController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public MessMemberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        [HttpGet("get-mess-member/{messId}")]
        public async Task<IActionResult> GetMessMember([FromRoute] int messId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmailClaim = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userEmailClaim))
                    return Unauthorized(ApiResponse<List<MessMemberDto>>.FailureResponse("Invalid token."));

                var userId = int.Parse(userIdClaim);
                var userEmail = userEmailClaim;
                //var messmembers = await _unitOfWork.MessMember.FindAsync(m => m.MessId == messId);
                var messmembers = await _unitOfWork.MessMember.GetAllIncluding(m => m.Mess);
                messmembers = messmembers.Where(m => m.MessId == messId);
                var loggedinMessMember = messmembers.FirstOrDefault(m => m.Email == userEmail);
                bool loggedinMessMemberCanEdit = false;
                if (loggedinMessMember != null)
                {
                    loggedinMessMemberCanEdit = loggedinMessMember.Role == "Manager" ? true : false;
                }
                var messMembersDto = messmembers.Select(m => new MessMemberDto
                {
                    MessMemberId = m.MessMemberId,
                    MessId = m.MessId,
                    Name = m.Name,
                    Email = m.Email,
                    Role = m.Role,
                    JoinedAt = m.JoinedAt,
                    CanEdit= (m.Mess.CreatedBy == userId) || (m.Email == userEmail) || loggedinMessMemberCanEdit
                }).ToList();
                return Ok(ApiResponse<List<MessMemberDto>>.SuccessResponse(messMembersDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<MessMemberDto>>.FailureResponse(ex.Message));
            }


        }
        [Authorize]
        [HttpGet("get-meals/{messId}")]
        public async Task<IActionResult> GetMessMemberMeals([FromRoute] int messId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmailClaim = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userEmailClaim))
                    return Unauthorized(ApiResponse<List<MealDto>>.FailureResponse("Invalid token."));

                var userId = int.Parse(userIdClaim);
                var userEmail = userEmailClaim;
                //var mess = await _unitOfWork.Mess.FirstOrDefaultAsync(m => m.MessId == messId);
                //var meals = await _unitOfWork.Meal.FindAsync(m => m.MessId == messId);
                var meals = await _unitOfWork.Meal.GetAllIncluding(m=>m.Mess,m=>m.MessMember);
                meals = meals.Where(m => m.MessId == messId);

                var loggedinMessMember = meals.FirstOrDefault(m => m.MessMember.Email == userEmail);
                bool loggedinMessMemberCanEdit = false;
                if (loggedinMessMember != null)
                {
                    loggedinMessMemberCanEdit = loggedinMessMember.MessMember.Role == "Manager" ? true : false;
                }
                var mealsDto = meals.Select(m => new MealDto
                {
                    MealId = m.MealId,
                    MessId = m.MessId,
                    MessMemberId = m.MessMemberId,
                    MealDate = m.MealDate,
                    Breakfast = m.Breakfast,
                    Lunch = m.Lunch,
                    Dinner = m.Dinner,
                    CanEdit= (m.Mess.CreatedBy == userId) || (m.MessMember.Email == userEmail) || loggedinMessMemberCanEdit
                }).ToList();
                return Ok(ApiResponse<List<MealDto>>.SuccessResponse(mealsDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<MealDto>>.FailureResponse(ex.Message));
            }


        }
        [Authorize]
        [HttpPost("update-meals")]
        public async Task<IActionResult> UpdateMeals([FromBody] MealDto meal)
        {
            if (meal == null)
            {
                return BadRequest(ApiResponse<MealDto>.FailureResponse("Meals is not updated."));
            }
            var meals = new Meal()
            {
                MealId = meal.MealId,
                MessId = meal.MessId,
                MessMemberId = meal.MessMemberId,
                MealDate = meal.MealDate,
                Breakfast = meal.Breakfast,
                Lunch = meal.Lunch,
                Dinner = meal.Dinner
            };
            _unitOfWork.Meal.Update(meals);
            var saveResult = await _unitOfWork.SaveAsync();
            if (!saveResult)
            {
                return BadRequest(ApiResponse<MealDto>.FailureResponse("Meals update failed."));
            }
            meal.MealId = meals.MealId;
            return Ok(ApiResponse<MealDto>.SuccessResponse(meal, "Meals updated successfully."));

        }
        [Authorize]
        [HttpGet("get-mess-member-summary/{messId}")]
        public async Task<IActionResult> GetMessMemberSummary([FromRoute] int messId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userId = int.Parse(userIdClaim);
                var messSummary = await _unitOfWork.MessMember.GetMessMemberSummaryAsync(messId, userId);
                return Ok(ApiResponse<List<MessMemberSummaryDto>>.SuccessResponse(messSummary));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<MessMemberSummaryDto>>.FailureResponse(ex.Message));
            }
        }
        [Authorize]
        [HttpPost("update_mess_member_info")]
        public async Task<IActionResult> UpdateMessMemberInfo([FromBody] MessMemberDto messMemberDto)
        {
            if (messMemberDto == null)
            {
                return BadRequest(ApiResponse<bool>.FailureResponse("Member is not updated."));
            }
            MessMember member;
            if (messMemberDto.MessMemberId > 0)
            {
                member = await _unitOfWork.MessMember.GetAsync(messMemberDto.MessMemberId);

                if (member == null)
                    return NotFound(ApiResponse<bool>.FailureResponse("Member not found."));

                member.Name = messMemberDto.Name;
                member.Email = messMemberDto.Email;
                member.Role = messMemberDto.Role?? "Member";
                member.Rent = messMemberDto.Rent;
            }
            else
            {
                var mess = await _unitOfWork.Mess.FirstOrDefaultAsync(m => m.MessId == messMemberDto.MessId);
                member = new MessMember
                {
                    MessId=messMemberDto.MessId,
                    Name = messMemberDto.Name,
                    Email = messMemberDto.Email,
                    Role = messMemberDto.Role ?? "Member",
                    Rent = messMemberDto.Rent,
                    JoinedAt = DateTime.Now
                };
                await _unitOfWork.MessMember.AddAsync(member);

                var meals = new List<Meal>();
                var firstDay = new DateOnly(mess.FromDate.Year, mess.FromDate.Month, mess.FromDate.Day);
                var lastDay = new DateOnly(mess.ToDate.Year, mess.ToDate.Month, mess.ToDate.Day);
                for (var date = firstDay; date <= lastDay; date = date.AddDays(1))
                {
                    meals.Add(new Meal
                    {
                        MessId= messMemberDto.MessId,
                        MessMember = member,
                        MealDate = date,
                        Breakfast = 0,
                        Lunch = 0,
                        Dinner = 0
                    });
                }
                _unitOfWork.Meal.AddRange(meals);
            }

            await _unitOfWork.SaveAsync();
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Member updated successfully!"));
        }
        [Authorize]
        [HttpPost("delete-mess-member")]
        public async Task<IActionResult> DeleteMessMember([FromBody] MessMemberDto member)
        {
            try 
            {
                if (member == null)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse("Member is not deleted."));
                }
                var messMember = await _unitOfWork.MessMember.GetAsync(member.MessMemberId);
                if (messMember == null)
                {
                    return NotFound(ApiResponse<bool>.FailureResponse("Member not found."));
                }
                _unitOfWork.MessMember.Remove(messMember);
                var meals = await _unitOfWork.Meal.FindAsync(m => m.MessMemberId == member.MessMemberId && m.MessId==member.MessId);
                if (meals?.Any() == true)
                    _unitOfWork.Meal.RemoveRange(meals);
                var costs = await _unitOfWork.MarketCost.FindAsync(c => c.MessMemberId == member.MessMemberId && c.MessId==member.MessId);
                if (costs?.Any() == true)
                    _unitOfWork.MarketCost.RemoveRange(costs);
                await _unitOfWork.SaveAsync();
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Member deleted successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.FailureResponse(ex.Message));
            }
        }
    }
}
