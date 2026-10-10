namespace BookingApp.Api.Auth;

public static class Roles
{
    public const string Admin = "admin";
    public const string Staff = "staff";
}

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string StaffOrAdmin = "StaffOrAdmin";
    public const string CanViewAppointment = "CanViewAppointment";
}

public static class AppClaims
{
    public const string StaffId = "staff_id";
}