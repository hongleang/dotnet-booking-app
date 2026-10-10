using BookingApp.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookingApp.Api.Auth;

public class AppointmentOwnerRequirement: IAuthorizationRequirement
{
}

public class AppointmentOwnerHandler : AuthorizationHandler<AppointmentOwnerRequirement, Appointment>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AppointmentOwnerRequirement requirement, Appointment resource)
    {
        if (context.User.IsInRole(Roles.Admin) || context.User.GetStaffId() == resource.StaffId)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}