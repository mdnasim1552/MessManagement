using MessApi.Models;
using MessApi.UnitOfWork;
using MessManagement.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                var messmembers = await _unitOfWork.MessMember.FindAsync(m => m.MessId == messId);
                var messMembersDto = messmembers.Select(m => new MessMemberDto
                {
                    MessMemberId = m.MessMemberId,
                    MessId = m.MessId,
                    Name = m.Name,
                    Email = m.Email,
                    Role = m.Role,
                    JoinedAt = m.JoinedAt
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
                var meals = await _unitOfWork.Meal.FindAsync(m => m.MessId == messId);
                var mealsDto = meals.Select(m => new MealDto
                {
                    MealId = m.MealId,
                    MessId = m.MessId,
                    MessMemberId = m.MessMemberId,
                    MealDate = m.MealDate,
                    Breakfast = m.Breakfast,
                    Lunch = m.Lunch,
                    Dinner = m.Dinner
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
                var messSummary = await _unitOfWork.MessMember.GetMessMemberSummaryAsync(messId);
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
            var member = await _unitOfWork.MessMember.GetAsync(messMemberDto.MessMemberId);

            if (member == null)
                return NotFound(ApiResponse<bool>.FailureResponse("Member not found."));

            if (!string.IsNullOrEmpty(messMemberDto.Name))
                member.Name = messMemberDto.Name;

            if (!string.IsNullOrEmpty(messMemberDto.Email))
                member.Email = messMemberDto.Email;

            if (!string.IsNullOrEmpty(messMemberDto.Role))
                member.Role = messMemberDto.Role;
            member.Rent = messMemberDto.Rent;

            await _unitOfWork.SaveAsync();
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Member updated successfully!"));
        }
    }
}
