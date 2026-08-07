using FortuneApplication.Dtos.Responses.Accounts;
using FortuneDomain.Entities;

namespace FortuneApplication.Mappers
{
    public static class AccountMappers
    {
        public static AccountResponse ToResponse(this Account account) =>
            new(account.Id, account.Name, account.Type, account.CreatedAt);
    }
}
