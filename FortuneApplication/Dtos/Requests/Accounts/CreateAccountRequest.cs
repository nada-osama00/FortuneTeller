using FortuneDomain.Enums;

namespace FortuneApplication.Dtos.Requests.Accounts
{
    public record CreateAccountRequest(string Name, AccountType Type);
}
