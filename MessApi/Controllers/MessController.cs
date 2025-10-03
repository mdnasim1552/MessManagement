using MessApi.Models;
using MessApi.UnitOfWork;
using MessManagement.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace MessApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public MessController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        [HttpPost("create-mess")]
        public async Task<IActionResult> CreateMess([FromBody] MessDto messDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID not found in token.");

            var userId = int.Parse(userIdClaim);

            var mess = new Mess
            {
                MessName = messDto.MessName,
                Description = messDto.Description,
                Month = messDto.Month,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,

                MessMembers = messDto.MessMembers.Select(m => new MessMember
                {
                    Name = m.Name,
                    Email = m.Email,
                    Role = m.Role ?? "Member",
                    JoinedAt = DateTime.UtcNow
                }).ToList(),
                CommonBills=messDto.CommonBills.Select(c=>new CommonBill
                {
                    BillType=c.BillType,
                    Amount=c.Amount
                }).ToList()
            };
            var meals = new List<Meal>();
            var firstDay = new DateOnly(messDto.Month.Year, messDto.Month.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            foreach (var member in mess.MessMembers)
            {
                for (var date = firstDay; date <= lastDay; date = date.AddDays(1))
                {
                    meals.Add(new Meal
                    {
                        Mess=mess,
                        MessMember = member,
                        MealDate = date,
                        Breakfast = 0,
                        Lunch = 0,
                        Dinner = 0
                    });
                }
            }

            // attach Meals
            mess.Meals = meals;

            await _unitOfWork.Mess.AddAsync(mess);
            var saveResult = await _unitOfWork.SaveAsync();
            //if (!saveResult)
            //{
            //    return BadRequest(new { message = "Mess creation failed.", result = false });
            //}
            //return Ok(new { message = "Mess created successfully.", result = true });

            if (!saveResult)
                return BadRequest(ApiResponse<string>.FailureResponse("Mess creation failed."));

            return Ok(ApiResponse<string>.SuccessResponse("Mess created successfully."));
        }
    }
}
