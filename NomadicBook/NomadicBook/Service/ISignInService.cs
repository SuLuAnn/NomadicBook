using NomadicBook.Dto;
using NomadicBook.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public interface ISignInService
    {
        public bool CanUseNickName(string nickname);

        public bool CanUseEmail(string email);

        public int SignUp(SignUpParameter singUpParameter);
        public SignInDto GetSignInData(SignInParameter singInParameter);
        public PresetAddressDto GetPresetAddress(short userId);
        public void SetPresetAddress(BookParameter bookData);
        public int SetPassword(short userId, PasswordParameter user);
        public int ForgetPassword(string email);
        public string RandomPassword();
        public bool IsVerify(short userId);
        public void SendVerify(string email);
        public void Verify(string email);

    }
}
