using FortuneDomain.Enums;

namespace FortuneApplication.Dtos.Requests.Accounts
{
    public record UpdateAccountRequest(string Name, AccountType Type);
}
