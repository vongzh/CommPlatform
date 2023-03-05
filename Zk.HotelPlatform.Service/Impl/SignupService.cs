using System;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SignupService : DataService<Signup>, ISignupService
    {
        public SignupService(IDataProvider<Signup> dataProvider) : base(dataProvider)
        {

        }


        public bool SaveSignup(Signup signup)
        {
            if (signup.Id <= 0)
            {
                signup.CreateTime = DateTime.Now;
                signup.IsDelete = (int)GlobalEnum.YESOrNO.N;
                var entity = base.AddAndGetEntity(signup);
                return entity != null && entity.Id > 0;
            }
            else
                throw new NotImplementedException();
        }
    }
}
