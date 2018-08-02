using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace VdChatBot.Dialogs
{
    [Serializable]
    public class BookSchedulingDialog : IDialog<object>
    {

        private List<Tuple<int, string>> Books = new List<Tuple<int, string>>()
        {
            new Tuple<int, string>(1, "X.Y.Z 1 "),
            new Tuple<int, string>(1, "X.Y.Z 2 "),
            new Tuple<int, string>(1, "X.Y.Z 3 "),
            new Tuple<int, string>(1, "X.Y.Z 4 ")
        };

        public Tuple<int, string> Book = null;

        public Task StartAsync(IDialogContext context)
        {
            MostrarMenu(context);

            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MostrarMenu(IDialogContext context)
        {
            var sb = new StringBuilder();

            foreach (var book in Books)
            {
                sb.AppendLine($"Código: {book.Item1} Livro: {book.Item2}");
            }

            await context.PostAsync($"Esse é o catalago de livros: \n{sb.ToString()}\n informe o código do livro desejado \n ou digite cancelar para cancelar a operação");
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {


            //var reply = context.MakeMessage();
            //reply.Type = ActivityTypes.Typing;
            //awa
            var activity = await result as Activity;

            int? code = null;

            if (activity.Text.ToLower().Contains("cancelar"))
            {
                await context.PostAsync("Operação cancelada");
                context.Done("cancelado");
                return;
            }

            try
            {
                code = Convert.ToInt32(activity.Text);
            }
            catch (Exception e)
            {
            }

            if (code.HasValue)
            {
                var book = Books.FirstOrDefault(x => x.Item1 == code.Value);
                if (book != null)
                {
                    await context.PostAsync($"Você deseja confirmar a reserva do livro \"{book.Item2}\"?");
                    Book = book;
                    context.Wait(ConfirmationBookMessageReceivedAsync);
                }
                else
                {
                    await context.PostAsync("Desculpe mas não consegui encontrar o livro pelo código informado, por favor informe outro código ou informe o código corretamente.");
                }

            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }


        }

        private async Task ConfirmationBookMessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {

            var activity = await result as Activity;
            if (activity.Text.ContainsAprovation())
            {
                await context.PostAsync($"Livro  \"{Book.Item2}\" reservado com sucesso!");
                context.Done(Book.Item2);
            }
            else if (activity.Text.ContainsNegation())
            {
                await context.PostAsync("Você não quis reserver o livro. desconsiderando o pedido de reserva!");
                context.Done(Book.Item2);
            }
            else
            {
                await context.PostAsync($"Você deseja confirmar a reserva do livro?");
                context.Wait(ConfirmationBookMessageReceivedAsync);
            }
        
        }
    }
}