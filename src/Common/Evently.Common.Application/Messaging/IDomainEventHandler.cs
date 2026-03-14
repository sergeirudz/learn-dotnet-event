using Evently.Common.Domain;
using MediatR;

namespace Evently.Common.Application.Messaging;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent; // Impose generic constraint to ensure TDomainEvent is a domain event
