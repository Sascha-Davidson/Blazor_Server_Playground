using Microsoft.AspNetCore.Components;
using Playground.Services;
using System.Web;

namespace Playground.FrontEnd.Components.Dialog
{
    public class DialogService : IDialogService
    {
        private readonly NavigationManager _navigation;

        public event Action<DialogInstance> OnDialogOpen;
        public event Action<DialogInstance> OnDialogClose;

        private DialogInstance _currentDialog;

        public DialogService(NavigationManager navigation)
        {
            _navigation = navigation;
        }

        public Task<TResult> ShowAsync<TDialog, TResult>(DialogParameters parameters = null)
            where TDialog : ComponentBase
        {
            _currentDialog = new DialogInstance
            {
                ComponentType = typeof(TDialog),
                Parameters = parameters ?? [],
                Completion = new TaskCompletionSource<object>()
            };

            OnDialogOpen?.Invoke(_currentDialog);
            UpdateUrl(typeof(TDialog).Name);

            return _currentDialog.Completion.Task.ContinueWith(t => (TResult)t.Result);
        }

        public Task ShowByTypeAsync(Type dialogType, DialogParameters parameters = null)
        {
            _currentDialog = new DialogInstance
            {
                ComponentType = dialogType,
                Parameters = parameters ?? [],
                Completion = new TaskCompletionSource<object>()
            };

            OnDialogOpen?.Invoke(_currentDialog);

            return Task.CompletedTask;
        }

        public void Close<TResult>(TResult result = default)
        {
            if (_currentDialog != null)
            {
                _currentDialog.Completion.TrySetResult(result);
                OnDialogClose?.Invoke(_currentDialog);
                _currentDialog = null;
                RemoveDialogFromUrl();
            }
        }

        private void UpdateUrl(string dialogName)
        {
            var uri = _navigation.ToAbsoluteUri(_navigation.Uri);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["dialog"] = dialogName;
            var baseUri = uri.GetLeftPart(UriPartial.Path);
            _navigation.NavigateTo($"{baseUri}?{query}", forceLoad: false, replace: true);
        }

        private void RemoveDialogFromUrl()
        {
            var uri = _navigation.ToAbsoluteUri(_navigation.Uri);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query.Remove("dialog");
            var baseUri = uri.GetLeftPart(UriPartial.Path);
            var newUri = query.Count > 0 ? $"{baseUri}?{query}" : baseUri;
            _navigation.NavigateTo(newUri, forceLoad: false, replace: true);
        }
    }
}
