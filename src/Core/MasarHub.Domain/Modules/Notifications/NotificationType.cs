namespace MasarHub.Domain.Modules.Notifications
{
    public enum NotificationType
    {
        InstructorRegistration,
        EmailConfirmation,
        PasswordChanged,
        TwoFactorEnabled,
        TwoFactorDisabled,
        RecoveryCodeUsed,
        CourseCreated,
        CourseApproval,
        OrderCreated,
        PaymentReceived
    }
}
