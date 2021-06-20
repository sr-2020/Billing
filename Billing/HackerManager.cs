using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public interface IHackerManager : IBaseRepository
    {

    }
    public class HackerManager: BaseEntityRepository, IHackerManager
    {
    }
}
