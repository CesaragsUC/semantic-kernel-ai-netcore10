using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OpenAI.Chat;
using System.Text;

namespace Demo.SematicKernel.Web;

[Route("api/[controller]")]
public class ChatController(Kernel _kernel) : Controller
{

    [HttpGet]
    [Route("question/{prompt}")]
    public async Task<IActionResult> Question(string prompt)
    {
        var chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();

        OllamaPromptExecutionSettings settings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var history = new ChatHistory();
        history.AddUserMessage(prompt);


        var response = await chatCompletion.GetChatMessageContentAsync(
            history,
            settings,
            _kernel
        );

        // Add the message from the agent to the chat history
        history.AddAssistantMessage(response.Content!);

        return Ok(response.Content);
    }

}
