namespace Lumeer.Models.Rest.Enums
{
    public enum NotificationType
    {
        ORGANIZATION_SHARED,
        PROJECT_SHARED,
        COLLECTION_SHARED,
        VIEW_SHARED,
        DUE_DATE_SOON,
        DUE_DATE_CHANGED,
        PAST_DUE_DATE,
        STATE_UPDATE,
        BULK_ACTION,
        TASK_ASSIGNED,
        TASK_UPDATED,
        TASK_REMOVED,
        TASK_UNASSIGNED,
        TASK_COMMENTED,
        TASK_MENTIONED,
        TASK_REOPENED,
        TASK_CHANGED // aggregated changes
    }

}
