using System.Collections.Generic;
using DSLNG.PEAR.Data.Persistence;
using StructureMap;

namespace DSLNG.PEAR.Services
{
    public class BaseService
    {
        private readonly IDataContext _dataContext;

        public BaseService(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        protected IDataContext DataContext
        {
            get { return _dataContext; }
        }

        protected IList<int> YearlyNumbers
        {
            get
            {
                var numbers = new List<int>();
                for (int i = 2015; i <= 2030; i++)
                {
                    numbers.Add(i);
                }

                return numbers;
            }
        }

        protected IList<int> YearlyNumbersForOperationData
        {
            get
            {
                var numbers = new List<int>();
                for (int i = 2011; i <= 2030; i++)
                {
                    numbers.Add(i);
                }

                return numbers;
            }
        }
    }
}
