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
        [HttpPost("update-current-mess")]
        public async Task<IActionResult> UpdateCurrentMess([FromBody] MessDto messDto)
        {
            if (messDto == null)
                return BadRequest(ApiResponse<bool>.FailureResponse("Current mess is not set."));

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmailClaim = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userEmailClaim))
                return Unauthorized(ApiResponse<bool>.FailureResponse("Invalid token."));

            var userId = int.Parse(userIdClaim);
            var userEmail = userEmailClaim;

            var user = await _unitOfWork.User.SingleOrDefaultAsync(u=>u.Id==userId);
            if (user == null)
                return NotFound(ApiResponse<bool>.FailureResponse("User not found."));

            user.CurrentMessId = messDto.MessId;
            //createdMesses.CurrentMess = true;

            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Current mess is set successfully!"));
        }
        [Authorize]
        [HttpPost("create-mess")]
        public async Task<IActionResult> CreateMess([FromBody] MessDto messDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(ApiResponse<int>.FailureResponse("User ID not found in token."));

            var userId = int.Parse(userIdClaim);

            var mess = new Mess
            {
                MessName = messDto.MessName,
                Description = messDto.Description,
                FromDate = messDto.FromDate,
                ToDate = messDto.ToDate,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,

                MessMembers = messDto.MessMembers.Select(m => new MessMember
                {
                    Name = m.Name,
                    Email = m.Email,
                    Rent=m.Rent,
                    Role = m.Role ?? "Member",
                    JoinedAt = DateTime.Now
                }).ToList(),
                CommonBills= (messDto.CommonBills ?? new List<CommonBillDto>()).Select(c=>new CommonBill
                {
                    BillType=c.BillType,
                    Amount=c.Amount
                }).ToList()
            };
            var meals = new List<Meal>();
            var firstDay = new DateOnly(messDto.FromDate.Year, messDto.FromDate.Month, messDto.FromDate.Day);
            var lastDay = new DateOnly(messDto.ToDate.Year, messDto.ToDate.Month, messDto.ToDate.Day);
            var users = await _unitOfWork.User.GetAllAsync();
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
                return BadRequest(ApiResponse<int>.FailureResponse("Mess creation failed."));
            // Now mess.MessId is available
            foreach (var u in users)
            {
                if (mess.MessMembers.Any(m => m.Email == u.Email))
                {
                    u.CurrentMessId = mess.MessId;
                }
            }

            await _unitOfWork.SaveAsync(); // Save the CurrentMessId updates

            return Ok(ApiResponse<int>.SuccessResponse(mess.MessId, "Mess created successfully."));
        }

        [Authorize]
        [HttpGet("get-mess")]
        public async Task<IActionResult> GetMess()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmailClaim = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userEmailClaim))
                    return Unauthorized(ApiResponse<List<MessDto>>.FailureResponse("Invalid token."));

                var userId = int.Parse(userIdClaim);
                var userEmail = userEmailClaim;
                var allUserMesses= await _unitOfWork.Mess.GetMessSummaryAsync(userId);
                //// Get all messes created by user
                //var createdMesses = await _unitOfWork.Mess.GetAllAsync();
                //var userCreatedMesses = createdMesses
                //    .Where(m => m.CreatedBy == userId)
                //    .ToList();

                //// Get all mess memberships by user's email
                //var allMembers = await _unitOfWork.MessMember.GetAllAsync();
                //var memberMessIds = allMembers
                //    .Where(mm => mm.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                //    .Select(mm => mm.MessId)
                //    .Distinct()
                //    .ToList();

                //// 4️⃣ Get messes where user is a member
                //var memberMesses = createdMesses
                //    .Where(m => memberMessIds.Contains(m.MessId))
                //    .ToList();

                //// 5️⃣ Combine both (remove duplicates)
                //var allUserMesses = userCreatedMesses
                //    .Union(memberMesses)
                //    .Select(m => new MessDto
                //    {
                //        MessId = m.MessId,
                //        MessName = m.MessName,
                //        Description = m.Description,
                //        Month = m.Month,
                //        IsCreatedByCurrentUser = m.CreatedBy == userId
                //    })
                //    .ToList();

                return Ok(ApiResponse<List<MessDto>>.SuccessResponse(allUserMesses));
            }
            catch(Exception ex)
            {
                return BadRequest(ApiResponse<List<MessDto>>.FailureResponse(ex.Message));
            }           
        }
        [Authorize]
        [HttpDelete("delete-mess/{messId}")]
        public async Task<IActionResult> DeleteMess([FromRoute] int messId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userId = int.Parse(userIdClaim);
                var mess = await _unitOfWork.Mess.FirstOrDefaultAsync(m => m.MessId == messId && m.CreatedBy == userId);
                if (mess == null)
                    return NotFound(ApiResponse<bool>.FailureResponse("Mess not found or you don't have permission to delete it."));
                _unitOfWork.Mess.Remove(mess);
                var saveResult = await _unitOfWork.SaveAsync();
                if (!saveResult)
                    return BadRequest(ApiResponse<bool>.FailureResponse("Failed to delete mess."));
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Mess deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.FailureResponse(ex.Message));
            }
        }

        [Authorize]
        [HttpGet("get-common-bills/{messId}")]
        public async Task<IActionResult> GetCommonBills([FromRoute] int messId)
        {
            try
            {
                var commonBills = await _unitOfWork.CommonBill.FindAsync(m => m.MessId == messId);
                var commonBillsDto = commonBills.Select(m => new CommonBillDto
                {
                    BillId = m.BillId,
                    MessId = m.MessId,
                    BillType = m.BillType,
                    Amount = m.Amount
                }).ToList();
                return Ok(ApiResponse<List<CommonBillDto>>.SuccessResponse(commonBillsDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<CommonBillDto>>.FailureResponse(ex.Message));
            }
        }

        [Authorize]
        [HttpDelete("delete-common-bills/{billId}")]
        public async Task<IActionResult> DeleteCommonBills([FromRoute] int billId)
        {
            try
            {
                var commonBills = await _unitOfWork.CommonBill.FirstOrDefaultAsync(m => m.BillId == billId);

                if (commonBills == null)
                    return NotFound(ApiResponse<bool>.FailureResponse("Common bill not found or you don't have permission to delete it."));
                _unitOfWork.CommonBill.Remove(commonBills);
                var saveResult = await _unitOfWork.SaveAsync();
                if (!saveResult)
                    return BadRequest(ApiResponse<bool>.FailureResponse("Failed to delete Common bill."));
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Common bill is deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<bool>>.FailureResponse(ex.Message));
            }
        }
        [Authorize]
        [HttpPost("update-and-save-common-bill")]
        public async Task<IActionResult> UpdateSaveCommonBill([FromBody] CommonBillDto billDto)
        {
            if (billDto == null)
                return BadRequest(ApiResponse<bool>.FailureResponse("Invalid bill data."));

            try
            {
                CommonBill bill;

                if (billDto.BillId > 0)
                {
                    // Existing bill: fetch from DB
                    bill = await _unitOfWork.CommonBill.FirstOrDefaultAsync(b => b.BillId == billDto.BillId);
                    if (bill == null)
                        return NotFound(ApiResponse<bool>.FailureResponse("Common bill not found."));

                    // Update properties
                    bill.MessId = billDto.MessId;
                    bill.BillType = billDto.BillType;
                    bill.Amount = billDto.Amount;

                    // Explicitly mark as updated
                    //_unitOfWork.CommonBill.Update(bill);
                }
                else
                {
                    // New bill: create
                    bill = new CommonBill
                    {
                        MessId = billDto.MessId,
                        BillType = billDto.BillType,
                        Amount = billDto.Amount
                    };
                    await _unitOfWork.CommonBill.AddAsync(bill);
                }

                // Save all changes
                var saveResult = await _unitOfWork.SaveAsync();
                
                if (!saveResult)
                    return BadRequest(ApiResponse<CommonBillDto>.FailureResponse("Failed to save common bill."));
                billDto.BillId = bill.BillId;
                return Ok(ApiResponse<CommonBillDto>.SuccessResponse(billDto, "Common bill saved successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CommonBillDto>.FailureResponse(ex.Message));
            }
        }
        [Authorize]
        [HttpGet("get-market-costs/{messId}")]
        public async Task<IActionResult> GetMarketCosts([FromRoute] int messId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmailClaim = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userEmailClaim))
                    return Unauthorized(ApiResponse<List<MarketCostDto>>.FailureResponse("Invalid token."));

                var userId = int.Parse(userIdClaim);
                var userEmail = userEmailClaim;
                //var marketCosts = await _unitOfWork.MarketCost.FindAsync(m => m.MessId == messId);
                var marketCosts = await _unitOfWork.MarketCost.GetAllIncluding(m => m.Mess, m => m.MessMember);
                marketCosts = marketCosts.Where(m => m.MessId == messId);

                var loggedinMessMember = marketCosts.FirstOrDefault(m => m.MessMember.Email == userEmail);
                bool loggedinMessMemberCanEdit = false;
                if (loggedinMessMember != null)
                {
                    loggedinMessMemberCanEdit = loggedinMessMember.MessMember.Role == "Manager" ? true : false;
                }
                var marketCostsDto = marketCosts.Select(m => new MarketCostDto
                {
                    CostId= m.CostId,
                    MessId = m.MessId,
                    MessMemberId = m.MessMemberId,
                    ExpenseDate = m.ExpenseDate.ToDateTime(TimeOnly.MinValue),
                    ProductName = m.ProductName,
                    Quantity = m.Quantity,
                    Unit = m.Unit,
                    Cost=m.Cost,
                    CanEdit = (m.Mess.CreatedBy == userId) || (m.MessMember.Email == userEmail) || loggedinMessMemberCanEdit
                }).ToList();
                return Ok(ApiResponse<List<MarketCostDto>>.SuccessResponse(marketCostsDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<MarketCostDto>>.FailureResponse(ex.Message));
            }
        }

        [Authorize]
        [HttpDelete("delete-market-costs/{costId}")]
        public async Task<IActionResult> DeleteMarketCosts([FromRoute] int costId)
        {
            try
            {
                var marketcosts = await _unitOfWork.MarketCost.FirstOrDefaultAsync(m => m.CostId == costId);

                if (marketcosts == null)
                    return NotFound(ApiResponse<bool>.FailureResponse("Common bill not found or you don't have permission to delete it."));
                _unitOfWork.MarketCost.Remove(marketcosts);
                var saveResult = await _unitOfWork.SaveAsync();
                if (!saveResult)
                    return BadRequest(ApiResponse<bool>.FailureResponse("Failed to delete Common bill."));
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Common bill is deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<bool>>.FailureResponse(ex.Message));
            }
        }
        [Authorize]
        [HttpGet("get-units")]
        public async Task<IActionResult> GetUnits()
        {
            try
            {

                var units = await _unitOfWork.Unit.GetAllAsync();
                var unitDto = units.Select(m => new UnitDto
                {
                    Id = m.Id,
                    Name=m.Name,
                    ShortName=m.ShortName
                }).ToList();
                return Ok(ApiResponse<List<UnitDto>>.SuccessResponse(unitDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<UnitDto>>.FailureResponse(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("update-and-save-market-costs")]
        public async Task<IActionResult> UpdateSaveMarketCosts([FromBody] MarketCostDto marketCostDto)
        {
            if (marketCostDto == null)
                return BadRequest(ApiResponse<MarketCostDto>.FailureResponse("Invalid market cost data."));

            try
            {
                MarketCost marketCost;
                if (marketCostDto.CostId > 0)
                {
                    // Existing bill: fetch from DB
                    marketCost = await _unitOfWork.MarketCost.FirstOrDefaultAsync(b => b.CostId == marketCostDto.CostId);
                    if (marketCost == null)
                        return NotFound(ApiResponse<MarketCostDto>.FailureResponse("market cost data not found."));

                    // Update properties
                    marketCost.MessId = marketCostDto.MessId;
                    marketCost.MessMemberId = marketCostDto.MessMemberId;
                    marketCost.ExpenseDate = DateOnly.FromDateTime(marketCostDto.ExpenseDate);
                    marketCost.ProductName = marketCostDto.ProductName;
                    marketCost.Quantity = marketCostDto.Quantity;
                    marketCost.Unit = marketCostDto.Unit;
                    marketCost.Cost = marketCostDto.Cost;

                    // Explicitly mark as updated
                    //_unitOfWork.MarketCost.Update(marketCost);
                }
                else
                {
                    // New bill: create
                    marketCost = new MarketCost
                    {
                        MessId = marketCostDto.MessId,
                        MessMemberId = marketCostDto.MessMemberId,
                        ExpenseDate = DateOnly.FromDateTime(marketCostDto.ExpenseDate),
                        ProductName = marketCostDto.ProductName,
                        Quantity = marketCostDto.Quantity,
                        Unit = marketCostDto.Unit,
                        Cost = marketCostDto.Cost

                    };
                    await _unitOfWork.MarketCost.AddAsync(marketCost);
                }

                // Save all changes
                var saveResult = await _unitOfWork.SaveAsync();

                if (!saveResult)
                    return BadRequest(ApiResponse<MarketCostDto>.FailureResponse("Failed to save market costs."));
                marketCostDto.CostId = marketCost.CostId;
                return Ok(ApiResponse<MarketCostDto>.SuccessResponse(marketCostDto, "Market cost is saved successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<MarketCostDto>.FailureResponse(ex.Message));
            }
        }
    }
}
