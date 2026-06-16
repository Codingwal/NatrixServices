using NatrixServices.Shared.Application;

namespace NatrixServices.Users.Application.Events;

public record CreateUserEvent(string Username) : IEvent;