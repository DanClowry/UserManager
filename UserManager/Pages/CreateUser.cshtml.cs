using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using UserManager.Models;

namespace UserManager.Pages
{
    public class CreateUserModel : PageModel
    {
        private readonly GraphServiceClient _graphServiceClient;

        public CreateUserModel(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public CreateUser NewUser { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (NewUser != null)
            {
                Console.WriteLine(NewUser.FirstName);
                var newUserRes = await _graphServiceClient.Users.PostAsync(new()
                {
                    AccountEnabled = false,
                    GivenName = NewUser.FirstName,
                    Surname = NewUser.LastName,
                    DisplayName = $"AUTO USER {NewUser.FirstName} {NewUser.LastName}",
                    MailNickname = $"AUTO.{NewUser.FirstName}.{NewUser.LastName}",
                    UserPrincipalName = $"AUTO.{NewUser.FirstName}.{NewUser.LastName}@example.com",
                    PasswordProfile = new()
                    {
                        ForceChangePasswordNextSignIn = true,
                        Password = "Password1"
                    }
                });
                var users = await _graphServiceClient.Users.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "displayName", "id" };
                });
                Console.WriteLine(users.Value[0].DisplayName);
            }

            return RedirectToPage("./Index");
        }
    }
}
