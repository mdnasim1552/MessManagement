using CommunityToolkit.Mvvm.ComponentModel;

namespace MessManagement.MVVM.ViewModels
{
    public partial class MessMemberModel:ObservableObject
    {
        public int MessMemberId { get; set; }
        public int MessId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }
        public DateTime? JoinedAt { get; set; }
        [ObservableProperty]
        public string heading = string.Empty;

    }
}
