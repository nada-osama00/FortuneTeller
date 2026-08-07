using FortuneDomain.Enums;

namespace FortuneApplication.Dtos.Responses.Accounts
{
    public record AccountResponse(Guid Id, string Name, AccountType Type, DateTime CreatedAt);
}
