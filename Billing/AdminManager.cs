﻿using Billing.Dto;
using Billing.Dto.Shop;
using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Billing
{
    public interface IAdminManager : IBaseRepository
    {
        List<ShopDto> GetShops(Expression<Func<ShopWallet, bool>> predicate);
        List<SpecialisationDto> GetSpecialisations(Expression<Func<Specialisation, bool>> predicate);
        List<CorporationDto> GetCorporations(Expression<Func<CorporationWallet, bool>> predicate);
        List<ProductTypeDto> GetProductTypes(Expression<Func<ProductType, bool>> predicate);
        List<NomenklaturaDto> GetNomenklaturas(Expression<Func<Nomenklatura, bool>> predicate);
        List<SkuDto> GetSkus(Expression<Func<Sku, bool>> predicate);
        List<UserDto> GetUsers(Expression<Func<SIN, bool>> predicate);

        ShopDto CreateOrUpdateShopWallet(int foreignKey, decimal amount, string name, int lifestyle, int owner);
        SpecialisationDto CreateOrUpdateSpecialisation(int id, int producttype, string name);
        NomenklaturaDto CreateOrUpdateNomenklatura(int id, string name, string code, int specialisationId, int lifestyle, decimal baseprice, int baseCount, string description, string pictureurl, int externalId = 0);
        SkuDto CreateOrUpdateSku(int id, int nomenklatura, int count, int corporation, string name, bool enabled, int externalId = 0);
        CorporationWallet CreateOrUpdateCorporationWallet(int id, decimal amount, string name, string logoUrl);

        void DeleteSpecialisation(int id);
        void DeleteNomenklatura(int id);
        void DeleteShop(int shopid);
        void DeleteSku(int skuId);

        void SetSpecialisation(int shopId, int specialisationId);
        void DropSpecialisation(int shopId, int specialisationId);
    }
    public class AdminManager : BaseBillingRepository, IAdminManager
    {
        public static string UrlNotFound = "";

        public List<ShopDto> GetShops(Expression<Func<ShopWallet, bool>> predicate)
        {
            return GetList(predicate, s => s.Owner.Sins, s => s.Wallet, s => s.Specialisations).Select(s =>
                      new ShopDto()
                      {
                          Id = s.Id,
                          Name = s.Name,
                          Lifestyle = ((Lifestyles)s.LifeStyle).ToString(),
                          Balance = BillingHelper.RoundDown(s.Wallet.Balance),
                          Specialisations = GetSpecialisationIds(s),
                          Owner = (s.Owner != null) ? new UserDto
                          {
                              ModelId = s.Owner.Model,
                              Name = s.Owner.GetActualSIN().PersonName
                          } : null
                      }).ToList();
        }

        public List<SpecialisationDto> GetSpecialisations(Expression<Func<Specialisation, bool>> predicate)
        {
            return GetList(predicate, s => s.ProductType).Select(s =>
                new SpecialisationDto(s)).ToList();
        }

        public List<CorporationDto> GetCorporations(Expression<Func<CorporationWallet, bool>> predicate)
        {
            return GetList(predicate, c => c.Wallet, c => c.Owner).Select(c =>
                    new CorporationDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        CorporationUrl = c.CorporationLogoUrl,
                        Owner = new UserDto
                        {
                            ModelId = c.Owner.CharacterId,
                            Name = c.Owner.PersonName
                        }
                    }).ToList();
        }

        public List<ProductTypeDto> GetProductTypes(Expression<Func<ProductType, bool>> predicate)
        {
            return GetList(predicate).Select(p =>
            new ProductTypeDto
            {
                Alias = p.Alias,
                Name = p.Name
            }).ToList();
        }

        public List<NomenklaturaDto> GetNomenklaturas(Expression<Func<Nomenklatura, bool>> predicate)
        {
            var list = GetListAsNoTracking(predicate, n => n.Specialisation.ProductType);
            return list.Select(s => new NomenklaturaDto(s)).ToList();
        }

        public List<SkuDto> GetSkus(Expression<Func<Sku, bool>> predicate)
        {
            var list = GetListAsNoTracking(predicate, s => s.Corporation, s => s.Nomenklatura, s => s.Nomenklatura.Specialisation.ProductType);
            return list.Select(s => new SkuDto(s)).ToList();
        }

        public List<UserDto> GetUsers(Expression<Func<SIN, bool>> predicate)
        {
            var list = GetListAsNoTracking(predicate, u => u.Character);
            return list.Select(c => new UserDto { Name = c.PersonName, ModelId = c.Character?.Model ?? 0 }).ToList();
        }

        public ShopDto CreateOrUpdateShopWallet(int shopId = 0, decimal amount = 0, string name = "default shop", int lifestyle = 1, int ownerId = 0)
        {
            ShopWallet shop = null;
            if (shopId > 0)
            {
                shop = Get<ShopWallet>(w => w.Id == shopId, s => s.Wallet);
                if (shop == null)
                {
                    throw new BillingException("shop not found");
                }
            }

            var owner = GetSINByModelId(ownerId);
            if (owner == null)
                throw new BillingException("owner not found");

            if (shopId == 0)
            {
                var newWallet = CreateOrUpdateWallet(WalletTypes.Shop);
                shop = new ShopWallet
                {
                    Wallet = newWallet,
                    Id = shopId,
                    OwnerId = ownerId
                };
                Add(shop);
            }
            shop.Name = name;
            shop.Wallet.Balance = amount;
            shop.LifeStyle = (int)BillingHelper.GetLifestyle(lifestyle);
            Context.SaveChanges();
            var dto = new ShopDto
            {
                Owner = new UserDto { ModelId = ownerId }
            };
            return dto;
        }

        public SpecialisationDto CreateOrUpdateSpecialisation(int id, int producttype, string name)
        {
            Specialisation specialisation = null;
            if (id > 0)
                specialisation = Get<Specialisation>(s => s.Id == id);
            else
            {
                specialisation = new Specialisation();
            }
            specialisation.Name = name;
            var product = Get<ProductType>(p => p.Id == producttype);
            if (product == null)
                throw new Exception("producttype not found");
            specialisation.ProductTypeId = producttype;
            Add(specialisation);
            SaveContext();
            var dbspec = Get<Specialisation>(s => s.Id == specialisation.Id, s=>s.ProductType);
            if (dbspec == null)
                throw new Exception("Ошибка добавления специализации");
            return new SpecialisationDto(dbspec);
        }

        public NomenklaturaDto CreateOrUpdateNomenklatura(int id, string name, string code, int specialisationId, int lifestyle, decimal baseprice, int basecount, string description, string pictureurl, int externalId = 0)
        {
            Nomenklatura nomenklatura = null;
            if (id > 0)
                nomenklatura = Get<Nomenklatura>(n => n.Id == id);
            if (nomenklatura == null)
            {
                nomenklatura = new Nomenklatura();
                nomenklatura.PictureUrl = UrlNotFound;
                nomenklatura.Code = string.Empty;
                Add(nomenklatura);
            }
            nomenklatura.Name = name;
            nomenklatura.Code = code;
            nomenklatura.BasePrice = baseprice;
            nomenklatura.Description = description;
            nomenklatura.PictureUrl = pictureurl;
            if (externalId != 0)
            {
                nomenklatura.ExternalId = externalId;
            }
            nomenklatura.BaseCount = basecount;
            Specialisation specialisation = null;
            if (specialisationId > 0)
                specialisation = Get<Specialisation>(p => p.Id == specialisationId);
            else
                specialisation = Get<Specialisation>(p => p.Id == nomenklatura.SpecialisationId);
            if (specialisation == null)
            {
                throw new BillingException("specialisation not found");
            }
            nomenklatura.SpecialisationId = specialisation.Id;
            nomenklatura.Lifestyle = lifestyle;
            nomenklatura.Lifestyle = (int)BillingHelper.GetLifestyle(nomenklatura.Lifestyle);
            Context.SaveChanges();
            nomenklatura = GetAsNoTracking<Nomenklatura>(n => n.Id == nomenklatura.Id, n => n.Specialisation.ProductType);
            if (nomenklatura == null)
                throw new Exception("Создать nomenklatura не получилось");
            return new NomenklaturaDto(nomenklatura);
        }

        public SkuDto CreateOrUpdateSku(int id, int nomenklaturaid, int count, int corporationid, string name, bool enabled, int externalId = 0)
        {
            Sku sku = null;
            if (id > 0)
                sku = Get<Sku>(s => s.Id == id);
            if (sku == null)
            {
                sku = new Sku();
                Add(sku);
            }
            sku.Enabled = enabled;
            if (count > 0)
                sku.Count = count;
            if (!string.IsNullOrEmpty(name))
                sku.Name = name;
            CorporationWallet corporation = null;
            if (corporationid > 0)
                corporation = Get<CorporationWallet>(c => c.Id == corporationid);
            else
                corporation = Get<CorporationWallet>(c => c.Id == sku.CorporationId);
            if (corporation == null)
            {
                throw new BillingException("Corporation not found");
            }

            sku.CorporationId = corporation.Id;
            if (externalId != 0)
            {
                sku.ExternalId = externalId;
            }
            Nomenklatura nomenklatura = null;
            if (nomenklaturaid > 0)
                nomenklatura = Get<Nomenklatura>(n => n.Id == nomenklaturaid);
            else
                nomenklatura = Get<Nomenklatura>(n => n.Id == sku.NomenklaturaId);
            if (nomenklatura == null)
            {
                throw new BillingException("Nomenklatura not found");
            }
            sku.NomenklaturaId = nomenklatura.Id;
            Context.SaveChanges();
            sku = GetAsNoTracking<Sku>(s => s.Id == sku.Id, s => s.Nomenklatura.Specialisation.ProductType);
            if (sku == null)
                throw new Exception("создать sku не получилось");
            return new SkuDto(sku);
        }

        public void DeleteSpecialisation(int id)
        {
            var nomenklaturas = GetList<Nomenklatura>(n => n.SpecialisationId == id);
            if (nomenklaturas != null && nomenklaturas.Count > 0)
            {
                throw new Exception($"Сперва необходимо удалить номенклатуры с id {string.Join(";", nomenklaturas)}");
            }
            Delete<Specialisation>(id);
        }

        public void DeleteNomenklatura(int id)
        {
            var skus = GetList<Sku>(s => s.NomenklaturaId == id);
            if (skus != null && skus.Count > 0)
            {
                throw new Exception($"Сперва необходимо удалить ску с id {string.Join(";", skus)}");
            }
            Delete<Nomenklatura>(id);
        }

        public void DeleteShop(int shopid)
        {
            var shop = Get<ShopWallet>(c => c.Id == shopid);
            if (shop == null)
                throw new Exception("shop not found");
            var specialisations = GetList<ShopSpecialisation>(s => s.ShopId == shopid);
            if (specialisations != null && specialisations.Count > 0)
            {
                throw new Exception("Сперва необходимо убрать все специализации магазина");
            }
            var contracts = GetList<Contract>(c => c.ShopId == shopid);
            if (contracts != null && contracts.Count > 0)
            {
                throw new Exception("Сперва необходимо убрать все контракты магазина");
            }
            var wallet = Get<Wallet>(w => w.Id == shop.WalletId);
            Remove(shop);
            Remove(wallet);
            SaveContext();
        }

        public void DeleteSku(int skuId)
        {
            var sku = Get<Sku>(s => s.Id == skuId);
            if (sku == null)
                throw new Exception("sku not found");
            Remove(sku);
            SaveContext();
        }

        public CorporationWallet CreateOrUpdateCorporationWallet(int corpId = 0, decimal amount = 0, string name = "unknown corporation", string logoUrl = "")
        {
            CorporationWallet corporation = null;
            if (corpId > 0)
                corporation = Get<CorporationWallet>(w => w.Id == corpId, c => c.Wallet);
            if (corporation == null)
            {
                var newWallet = CreateOrUpdateWallet(WalletTypes.Corporation);
                corporation = new CorporationWallet
                {
                    Wallet = newWallet,
                    Id = corpId,
                    CorporationLogoUrl = UrlNotFound
                };
                Add(corporation);
            }
            if (!string.IsNullOrEmpty(logoUrl))
                corporation.CorporationLogoUrl = logoUrl;
            if (!string.IsNullOrEmpty(name))
                corporation.Name = name;
            if (amount > 0)
                corporation.Wallet.Balance = amount;
            SaveContext();
            return corporation;
        }

        public void SetSpecialisation(int shopId, int specialisationId)
        {
            var shopspecialisation = Get<ShopSpecialisation>(s => s.ShopId == shopId && s.SpecialisationId == specialisationId);
            if (shopspecialisation != null)
                return;
            var shop = Get<ShopWallet>(s => s.Id == shopId);
            var specialisation = Get<Specialisation>(s => s.Id == specialisationId);
            if (shop == null || specialisation == null)
                throw new Exception($"некорректные входные данные shop: {shop?.Id}, specialisation: {specialisation?.Id} ");
            Add(new ShopSpecialisation { ShopId = shopId, SpecialisationId = specialisationId });
            SaveContext();
        }

        public void DropSpecialisation(int shopId, int specialisationId)
        {
            var specialisation = Get<ShopSpecialisation>(s => s.ShopId == shopId && s.SpecialisationId == specialisationId);
            if (specialisation != null)
            {
                Remove(specialisation);
                SaveContext();
            }
        }

        private List<int> GetSpecialisationIds(ShopWallet shop)
        {
            var list = new List<int>();
            if (shop.Specialisations == null)
                return list;
            list.AddRange(shop.Specialisations.Select(s => s.SpecialisationId));
            return list;
        }
    }



}