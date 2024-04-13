using prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Res;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity
{
    public class Payload
    {
        //使用者資訊
        public EmpResDTO? info { get; set; }
        //過期時間
        public int exp { get; set; }
    }
}
