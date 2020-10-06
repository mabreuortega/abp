﻿using System;
using System.Text;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.WebAssembly;

namespace Volo.Abp.BlazoriseUI.Components
{
    public partial class UiMessageAlert : ComponentBase, IDisposable
    {
        private object messageIcon;

        protected override void OnInitialized()
        {
            UiMessageNotifierService.MessageReceived += OnMessageReceived;

            base.OnInitialized();
        }

        private void OnMessageReceived(object sender, UiMessageEventArgs e)
        {
            MessageType = e.MessageType;
            Message = e.Message;
            Title = e.Title;
            Callback = e.Callback;

            ModalRef.Show();
        }

        public void Dispose()
        {
            if (UiMessageNotifierService != null)
            {
                UiMessageNotifierService.MessageReceived -= OnMessageReceived;
            }
        }

        protected Task OnOkClicked()
        {
            ModalRef.Hide();

            return Okayed.InvokeAsync(null);
        }

        protected Task OnConfirmClicked()
        {
            ModalRef.Hide();

            if (IsConfirmation && Callback != null)
            {
                Callback.SetResult(true);
            }

            return Confirmed.InvokeAsync(null);
        }

        protected Task OnCancelClicked()
        {
            ModalRef.Hide();

            if (IsConfirmation && Callback != null)
            {
                Callback.SetResult(false);
            }

            return Canceled.InvokeAsync(null);
        }

        protected Modal ModalRef { get; set; }

        protected virtual bool IsConfirmation
            => MessageType == UiMessageType.Confirmation;

        protected virtual bool ShowMessageIcon
           => Options?.ShowMessageIcon ?? true;

        protected virtual object MessageIcon => MessageType switch
        {
            UiMessageType.Info => IconName.Info,
            UiMessageType.Success => IconName.Check,
            UiMessageType.Warning => IconName.Exclamation,
            UiMessageType.Error => IconName.Stop,
            UiMessageType.Confirmation => IconName.QuestionCircle,
            _ => null,
        };

        protected virtual string MessageIconColor => MessageType switch
        {
            UiMessageType.Info => "#0000ff",
            UiMessageType.Success => "#00ff00",
            UiMessageType.Warning => "#ffae00",
            UiMessageType.Error => "#e8301c",
            UiMessageType.Confirmation => "#de692f",
            _ => null,
        };

        protected virtual string MessageIconStyle
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append($"color:{MessageIconColor}");

                return sb.ToString();
            }
        }

        protected virtual string OkButtonText
            => Options?.OkButtonText ?? "OK";

        protected virtual string ConfirmButtonText
            => Options?.ConfirmButtonText ?? "Confirm";

        protected virtual string CancelButtonText
            => Options?.CancelButtonText ?? "Cancel";

        [Parameter] public UiMessageType MessageType { get; set; }

        [Parameter] public string Title { get; set; }

        [Parameter] public string Message { get; set; }

        [Parameter] public TaskCompletionSource<bool> Callback { get; set; }

        [Parameter] public UiMessageOptions Options { get; set; }

        [Parameter] public EventCallback Okayed { get; set; } // TODO: ?

        [Parameter] public EventCallback Confirmed { get; set; }

        [Parameter] public EventCallback Canceled { get; set; }

        [Inject] protected IUiMessageNotifierService UiMessageNotifierService { get; set; }
    }
}
