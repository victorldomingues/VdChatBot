using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace VdChatBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;

        }


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {


            var activity = await result as Activity;

            var text = activity.Text.ToLower();

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            if (text.ContainsAprovation())
            {
                await context.PostAsync($"Certo vou te redirecionar para o catalogo de livros");
                context.Call(new BookSchedulingDialog(), this.RootDialogResumeAfter);

            }
            else if (text.ContainsNegation())
            {
                await context.PostAsync(
                    "Ok, você não quer fazer uma reserva do livro. caso mude de ideia digite \"sim\".");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                // return our reply to the user
                await context.PostAsync(
                    $"Digite \"sim\" case queira reservar um livro comigo, ou \"não\" caso não queira reservar");
                context.Wait(MessageReceivedAsync);
            }


        }

        private async Task RootDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {

            var resultFromNewOrder = await result;

            await context.PostAsync($"O reserva do livro: {resultFromNewOrder} foi realizado com sucesso!");

            // Again, wait for the next message from the user.
            context.Wait(this.MessageReceivedAsync);

        }
    }
}