using MasarHub.Application.Common.Events;
using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Events;
using MasarHub.Infrastructure.Persistence.Configurations.Identity;
using MasarHub.Infrastructure.Persistence.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MasarHub.Infrastructure.Persistence.Contexts
{
    public class MasarHubDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly IPublisher _publisher;
        public MasarHubDbContext(DbContextOptions<MasarHubDbContext> options, IPublisher publisher)
            : base(options)
        {
            _publisher = publisher;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(IPersistenceAssemblyMarker).Assembly);
            builder.ApplyIdentitySchema();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var (entities, domainEvents) = GetDomainEventsWithEntities();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (!domainEvents.Any())
                return result;

            await DispatchDomainEventsAsync(domainEvents, cancellationToken);
            entities.ForEach(entity => entity.ClearDomainEvents());

            return result;
        }

        private (List<BaseEntity> Entities, IReadOnlyList<IDomainEvent> Events) GetDomainEventsWithEntities()
        {
            var entities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var events = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            return (entities, events);
        }
        private async Task DispatchDomainEventsAsync(IReadOnlyList<IDomainEvent> domainEvents, CancellationToken ct)
        {
            foreach (var domainEvent in domainEvents)
            {
                var type = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
                var notification = Activator.CreateInstance(type, domainEvent) as INotification
                    ?? throw new InvalidOperationException($"Cannot create notification for: {domainEvent.GetType().Name}");

                await _publisher.Publish(notification, ct);
            }

            #region parallel 
            //await Task.WhenAll(domainEvents.Select(domainEvent =>
            //{
            //    var type = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            //    var notification = Activator.CreateInstance(type, domainEvent) as INotification
            //        ?? throw new InvalidOperationException($"Cannot create notification for: {domainEvent.GetType().Name}");

            //    return _publisher.Publish(notification, ct);
            //}));
            #endregion
        }
    }
}
