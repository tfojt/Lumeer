using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Services;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.ViewModels
{
    public class TaskCommentsViewModel : BaseViewModel
    {
        public delegate void ScrollRequestedEventHandler(TaskCommentItem comment, ScrollToPosition position, bool animated);
        public event ScrollRequestedEventHandler ScrollRequested;

        private readonly IAlertService _alertService;

        public ObservableCollection<TaskCommentItem> Comments { get; set; } = new ObservableCollection<TaskCommentItem>();

        private bool _loadingComments;
        public bool LoadingComments
        {
            get => _loadingComments;
            set => SetValue(ref _loadingComments, value);
        }
        
        private string _newCommentText;
        public string NewCommentText 
        {
            get => _newCommentText;
            set
            {
                SetValue(ref _newCommentText, value);
                CanSendComment = !string.IsNullOrEmpty(NewCommentText);
            }
        }

        private bool _canSendComment;
        public bool CanSendComment
        {
            get => _canSendComment;
            set
            {
                SetValue(ref _canSendComment, value);
                SendCommentColor = _canSendComment ? Color.Black : Color.Gray;
            }
        }

        private Color _sendCommentColor = Color.Gray;
        public Color SendCommentColor
        {
            get => _sendCommentColor;
            set => SetValue(ref _sendCommentColor, value);
        }

        private bool _editingComment;
        public bool EditingComment
        {
            get => _editingComment;
            set => SetValue(ref _editingComment, value);
        }

        private TaskCommentItem _editedTaskCommentItem;
        private Models.Rest.Task _task;

        public IAsyncCommand LoadCommentsCmd => new AsyncCommand(LoadComments, allowsMultipleExecutions: false);
        public IAsyncCommand SendCommentCmd => new AsyncCommand(SendComment, allowsMultipleExecutions: false);
        public ICommand EndEditingCommentCmd => new Command(EndEditingComment);
        public ICommand StartEditingCommentCmd => new Command<TaskCommentItem>(StartEditingComment);
        public ICommand DeleteCommentCmd => new Command<TaskCommentItem>(async (TaskCommentItem c) => await DeleteComment(c));

        public TaskCommentsViewModel(Models.Rest.Task task)
        {
            _task = task;

            _alertService = DependencyService.Get<IAlertService>();

            Task.Run(LoadComments);
        }

        private async Task LoadComments()
        {
            LoadingComments = true;

            try
            {
                var comments = await ApiClient.Instance.GetComments(_task);

                Comments.Clear();
                
                int lastIndex = comments.Count - 1;
                for (int i = lastIndex; i >= 0; i--)
                {
                    var comment = comments[i];
                    var commentItem = new TaskCommentItem(comment);
                    Comments.Add(commentItem);
                }

                if (Comments.Count > 0)
                {
                    var lastComment = Comments[lastIndex];
                    ScrollRequested?.Invoke(lastComment, ScrollToPosition.End, true);
                }
            }
            catch (Exception ex)
            {
                var message = "Could not load comments";
                await _alertService.DisplayAlert("Error", message, "Ok", ex);
            }
            finally
            {
                LoadingComments = false;
            }
        }

        private async Task SendComment()
        {
            if (EditingComment)
            {
                try
                {
                    var editedTaskComment = new EditedTaskComment(_editedTaskCommentItem.TaskComment, NewCommentText);
                    TaskComment updatedTaskComment = await ApiClient.Instance.EditComment(editedTaskComment);

                    var updatedTaskCommentItem = new TaskCommentItem(updatedTaskComment);
                    _editedTaskCommentItem.UpdateData(updatedTaskCommentItem);

                    EndEditingComment();
                }
                catch (Exception ex)
                {
                    var message = "Could not edit comment";
                    await _alertService.DisplayAlert("Error", message, "Ok", ex);
                }
            }
            else
            {
                //ApiClient.Instance.SendComment();
                //await RefreshTaskData();
            }
        }
        
        private void EndEditingComment()
        {
            _editedTaskCommentItem = null;
            EditingComment = false;
            NewCommentText = null;
        }

        private void StartEditingComment(TaskCommentItem taskCommentItem)
        {
            _editedTaskCommentItem = taskCommentItem;
            EditingComment = true;
            NewCommentText = taskCommentItem.TaskComment.Comment;
        }

        private async Task DeleteComment(TaskCommentItem taskCommentItem)
        {
            try
            {
                await ApiClient.Instance.DeleteComment(taskCommentItem.TaskComment);

                Comments.Remove(taskCommentItem);

                if (EditingComment && taskCommentItem == _editedTaskCommentItem)
                {
                    EndEditingComment();
                }
            }
            catch (Exception ex)
            {
                var message = "Could not delete comment";
                await _alertService.DisplayAlert("Error", message, "Ok", ex);
                return;
            }

            await RefreshTaskData();
        }

        private async Task RefreshTaskData()
        {
            try
            {
                var tasks = await ApiClient.Instance.GetActualTasks(_task.Id);
                _task.UpdateData(tasks.Single());
            }
            catch (Exception ex)
            {
                var message = "Could not get actual task data";
                await _alertService.DisplayAlert("Error", message, "Ok", ex);
                return;
            }
        }
    }
}
