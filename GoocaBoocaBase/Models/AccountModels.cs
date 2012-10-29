using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GoocaBoocaBase.Models
{

    #region モデル

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "現在のパスワード")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "新しいパスワード")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "新しいパスワードの確認入力")]
        [Compare("NewPassword", ErrorMessage = "新しいパスワードと確認のパスワードが一致しません。")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "ユーザー名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }

        [Display(Name = "このアカウントを記憶する")]
        public bool RememberMe { get; set; }
    }


    public class RegisterModel
    {
        [Required]
        [Display(Name = "ユーザー名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "電子メール アドレス")]
        public string Email { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "パスワードの確認入力")]
        [Compare("Password", ErrorMessage = "パスワードと確認のパスワードが一致しません。")]
        public string ConfirmPassword { get; set; }
    }
    #endregion

    #region Services
    // FormsAuthentication 型はシールされていて静的メンバーを含むため、そのメンバーを呼び出す
    // コードの単体テストを実行することは困難です。次のインターフェイスとヘルパー クラスは、
    // AccountController コードの単体テストを実行できるようにするために、そのような型の抽象ラッパーを
    // 作成する方法を示しています。

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("値を null または空にすることはできません。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("値を null または空にすることはできません。", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("値を null または空にすることはできません。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("値を null または空にすることはできません。", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("値を null または空にすることはできません。", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("値を null または空にすることはできません。", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("値を null または空にすることはできません。", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("値を null または空にすることはできません。", "newPassword");

            // 特定のエラー シナリオでは、基になる ChangePassword() は
            // false を返す代わりに例外をスローします。
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("値を null または空にすることはできません。", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // すべてのステータス コードの一覧については、http://go.microsoft.com/fwlink/?LinkID=177550 を
            // 参照してください。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "このユーザー名は既に存在します。別のユーザー名を入力してください。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "その電子メール アドレスのユーザー名は既に存在します。別の電子メール アドレスを入力してください。";

                case MembershipCreateStatus.InvalidPassword:
                    return "指定されたパスワードは無効です。有効なパスワードの値を入力してください。";

                case MembershipCreateStatus.InvalidEmail:
                    return "指定された電子メール アドレスは無効です。値を確認してやり直してください。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "パスワードの回復用に指定された回答が無効です。値を確認してやり直してください。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "パスワードの回復用に指定された質問が無効です。値を確認してやり直してください。";

                case MembershipCreateStatus.InvalidUserName:
                    return "指定されたユーザー名は無効です。値を確認してやり直してください。";

                case MembershipCreateStatus.ProviderError:
                    return "認証プロバイダーからエラーが返されました。入力を確認してやり直してください。問題が解決しない場合は、システム管理者に連絡してください。";

                case MembershipCreateStatus.UserRejected:
                    return "ユーザーの作成要求が取り消されました。入力を確認してやり直してください。問題が解決しない場合は、システム管理者に連絡してください。";

                default:
                    return "不明なエラーが発生しました。入力を確認してやり直してください。問題が解決しない場合は、システム管理者に連絡してください。";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "'{0}' の長さは {1} 文字以上である必要があります。";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minCharacters, int.MaxValue)
            };
        }
    }
    #endregion

}
