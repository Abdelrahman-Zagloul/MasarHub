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
        CourseSubmittedForApproval,
        CourseApproved,
        CourseRejected,
        OrderCreated,
        OrderCancelled,
        PaymentReceived,
        PaymentFailed,
        CourseEnrollmentCreated
    }
}
