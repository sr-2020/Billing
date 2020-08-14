using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Billing
{
    public class BaseBillingRepository : BaseEntityRepository
    {
        protected ISettingsManager _settings = IocContainer.Get<ISettingsManager>();

        protected string GetWalletName(Wallet wallet)
        {
            if (wallet == null)
                return string.Empty;
            switch (wallet.WalletType)
            {
                case (int)WalletTypes.Character:
                    var sin = Get<SIN>(s => s.WalletId == wallet.Id);
                    if (sin == null)
                        return string.Empty;
                    return $"{sin.CharacterId} {sin.PersonName} {sin.Sin}";
                case (int)WalletTypes.Corporation:
                    var corp = Get<CorporationWallet>(c => c.WalletId == wallet.Id);
                    if (corp == null)
                        return string.Empty;
                    return $"{corp.Id}";
                case (int)WalletTypes.Shop:
                    var shop = Get<ShopWallet>(c => c.WalletId == wallet.Id);
                    if (shop == null)
                        return string.Empty;
                    return $"{shop.Id} {shop.Name}";
                case (int)WalletTypes.MIR:
                    return "MIR";
                default:
                    return string.Empty;
            }
        }

        protected TransferDto CreateTransferDto(Transfer transfer, TransferType type, string owner = "владелец кошелька")
        {
            return new TransferDto
            {
                Comment = transfer.Comment,
                TransferType = type.ToString(),
                Amount = transfer.Amount,
                NewBalance = type == TransferType.Incoming ? transfer.NewBalanceTo : transfer.NewBalanceFrom,
                OperationTime = transfer.OperationTime,
                From = type == TransferType.Incoming ? GetWalletName(transfer.WalletFrom) : owner,
                To = type == TransferType.Incoming ? owner : GetWalletName(transfer.WalletTo),
                Anonimous = transfer.Anonymous
            };
        }


    }
}
