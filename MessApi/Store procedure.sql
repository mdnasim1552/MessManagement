USE [MessManagementDb]
GO
/****** Object:  StoredProcedure [dbo].[GET_MESS_BY_USER]    Script Date: 6/10/2026 11:33:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GET_MESS_BY_USER](
	@UserId int
)
AS
BEGIN
WITH
  Market AS (
    SELECT MessId, SUM(Cost)          AS TotalMarketCost
    FROM MarketCosts
    GROUP BY MessId
  ),
  Meal AS (
    SELECT MessId, SUM(Breakfast + Lunch + Dinner) AS TotalMeals
    FROM Meals
    GROUP BY MessId
  ),
  Common AS (
    SELECT MessId, SUM(Amount)        AS TotalCommonBill
    FROM CommonBills
    GROUP BY MessId
  ),
  Members AS (
    SELECT MessId, COUNT(*) AS MemberCount,STRING_AGG(Name, ', ') AS MemberNames
    FROM MessMembers
    GROUP BY MessId
  ),
  Messes as(
	SELECT DISTINCT m.MessId, m.MessName, m.[Description], m.[FromDate], m.[ToDate], m.CreatedBy, m.CreatedAt
	FROM Mess m
	WHERE m.CreatedBy = @UserId
	UNION
	SELECT m.MessId, m.MessName, m.Description, m.[FromDate], m.[ToDate], m.CreatedBy,m.CreatedAt
	FROM MessMembers mm
	INNER JOIN Mess m ON mm.MessId = m.MessId
	WHERE mm.Email = (select Email from Users where id=@UserId)
  )
SELECT
  m.MessId,
  m.MessName,
  mem.MemberNames,
  m.[Description],
  m.[FromDate],
  m.[ToDate],
  m.CreatedBy,
  m.CreatedAt,
  CurrentMess=IIF(m.MessId = u.CurrentMessId, CAST(1 AS bit), CAST(0 AS bit)) ,
  IsCreatedByCurrentUser=iif(m.CreatedBy=@UserId,cast(1 as bit),cast(0 as bit)),
  COALESCE(ma.TotalMarketCost, 0) AS TotalMarketCost,
  COALESCE(me.TotalMeals, 0) AS TotalMeals,
  CASE WHEN COALESCE(me.TotalMeals, 0) = 0 THEN 0
       ELSE COALESCE(ma.TotalMarketCost, 0) / me.TotalMeals
  END AS MealRate,
  CASE WHEN COALESCE(mem.MemberCount, 0) = 0 THEN 0
       ELSE COALESCE(co.TotalCommonBill, 0) / mem.MemberCount
  END AS CommonBillPerMember
FROM Messes m
LEFT JOIN Market ma ON m.MessId = ma.MessId
LEFT JOIN Meal me   ON m.MessId = me.MessId
LEFT JOIN Common co  ON m.MessId = co.MessId
LEFT JOIN Members mem ON m.MessId = mem.MessId
LEFT JOIN Users u on u.Id = @UserId
ORDER BY m.MessId;
END
GO
/****** Object:  StoredProcedure [dbo].[GetMessMemberSummary]    Script Date: 6/10/2026 11:33:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	CREATE procedure [dbo].[GetMessMemberSummary]
	(
		@MessId INT,
		@UserId int
	)
	AS
	BEGIN
		 declare @TotalMarketCost decimal(10, 2)=(select sum(Cost) from MarketCosts where MessId=@MessId)
		 declare @TotalMeals decimal(10, 2)=(select sum(Breakfast+Lunch+Dinner) from Meals where MessId=@MessId)
		 declare @MealRate decimal(10, 2)=iif(COALESCE(@TotalMeals, 0) = 0,0, COALESCE(@TotalMarketCost,0)/@TotalMeals)
		 declare @Commonbill decimal(10, 2)=COALESCE((select sum(Amount) from CommonBills where MessId=@MessId),0)/(select count(MessMemberId) from MessMembers where MessId=@MessId)
		 --select TotalMarketCost=@TotalMarketCost,TotalMeals=@TotalMeals, MealRate=@MealRate,Commonbill=@Commonbill
		 declare @CreatedBy int=(select CreatedBy from Mess where MessId=@MessId)
		 declare @UserEmail nvarchar(255)=(select Email from Users where id=@UserId)
		 declare @UserRole nvarchar(20)=(select [Role] from MessMembers where MessId=@MessId and Email=@UserEmail)
		   --select sum(Breakfast+Lunch+Dinner) from Meals where MessId=10 group by 

		 select 
		 m.MessMemberId,
		 MessId=@MessId,
		 m.[Name],
		 MealRate=@MealRate,
		 m.TotalMeal,
		 TotalMealCost=isnull(m.TotalMealCost,0),
		 MarketCost=isnull(n.MarketCost,0),
		 GetOrPayFromMeal=isnull(m.TotalMealCost,0)-isnull(n.MarketCost,0),
		 Rent=isnull(o.Rent,0),
		 TotalHaveToPay=isnull(@Commonbill,0)+isnull(o.Rent,0)+(isnull(m.TotalMealCost,0)-isnull(n.MarketCost,0)),
		 o.[Role],
		 o.Email,
		 IsCreatedByCurrentUser=iif(@CreatedBy=@UserId,cast(1 as bit),iif(@UserRole='Manager',cast(1 as bit),cast(0 as bit)))
		 from (  select b.MessMemberId, b.Name,TotalMeal=sum(Breakfast+Lunch+Dinner), TotalMealCost= sum(Breakfast+Lunch+Dinner)*@MealRate from Meals a 
		 		inner join MessMembers b on a.MessMemberId=b.MessMemberId and a.MessId=b.MessId
		 		where a.MessId=@MessId group by b.Name,b.MessMemberId) m 
		 left join (select MessMemberId,MarketCost=sum(cost) from MarketCosts where MessId=@MessId group by MessMemberId) n on m.MessMemberId=n.MessMemberId
		 left join MessMembers o on o.MessMemberId=m.MessMemberId and o.MessId=@MessId
	END
GO
