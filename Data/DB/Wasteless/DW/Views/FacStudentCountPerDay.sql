 CREATE VIEW [DW].[FacStudentCountPerDay] as 
  Select DateSID, SchoolYear, sum(c.Boys) + sum(c.Girls) StudentCountTotal, LocationSID  from DW.DimDate a
  join DW.DimSchoolYear b on  a.Date between b.AutumnPeriodStart and b.SpringPeriodEnd
  join DW.FacStudentCount c on b.SchoolYearSID = c.SchoolYearSID
  group by DateSID, SchoolYear, LocationSID