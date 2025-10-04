using AppPhimLo.Models;
using AppPhimLo.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;

namespace AppPhimLo.ViewModels
{
    public class CommentItem : BindableObject
    {
        public int? UserId { get; set; }
        public string? Slug { get; set; }

        public string? DisplayName { get; set; }
        public string? Content { get; set; }
        public string? TimeAgo { get; set; } = "vừa xong";
    }

    public class PlayerViewModel : BindableObject
    {
        // --- State / Data ---


        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set { isLoading = value; OnPropertyChanged(); }
        }

        private string? errorMessage;
        public string? ErrorMessage
        {
            get => errorMessage;
            set { errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
        }
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        private string? episodeTitle;
        public string? EpisodeTitle
        {
            get => episodeTitle;
            set { episodeTitle = value; OnPropertyChanged(); }
        }

        public string? MovieSlug { get; private set; }
        int CurrentUserId = 1;
        private string? webPlayerUrl; // link_embed
        public string? WebPlayerUrl
        {
            get => webPlayerUrl;
            set { webPlayerUrl = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasEmbed)); }
        }
        public bool HasEmbed => !string.IsNullOrWhiteSpace(WebPlayerUrl);

        // --- Like ---
        private bool isLiked;
        public bool IsLiked
        {
            get => isLiked;
            set {
                if (isLiked != value)
                {
                    isLiked = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(LikeButtonText));
                    OnPropertyChanged(nameof(LikeButtonColor));
                }
            }
        }

        private int likesCount;
        public int LikesCount
        {
            get => likesCount;
            set { likesCount = value; OnPropertyChanged(); }
        }

        // --- Comments ---
        public ObservableCollection<CommentItem> Comments { get; } = new();
        public int CommentsCount => Comments.Count;

        private string? newCommentText;
        public string? NewCommentText
        {
            get => newCommentText;
            set { newCommentText = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPost)); }
        }
        public bool CanPost => !string.IsNullOrWhiteSpace(NewCommentText);
        public bool IsPosting { get; private set; }


        // 🩷 Text hiển thị nút (binding thẳng)
        public string LikeButtonText => IsLiked ? "❤️ Đã thích" : "♡ Thích";

        // 🎨 Màu nút (binding thẳng)
        public Color LikeButtonColor => IsLiked ? Colors.Red : Colors.Gray;



        private MovieService _service;

        // --- Commands ---
        public ICommand ToggleLikeCommand { get; }
        public ICommand ShareCommand { get; }
        public ICommand PostCommentCommand { get; }

        public PlayerViewModel(MovieService service)
        {
            _service =  service;
            ToggleLikeCommand = new Command(async  () =>
            {
                await ToggleFavoriteAsync();
            });

            ShareCommand = new Command(async () =>
            {
                if (string.IsNullOrWhiteSpace(WebPlayerUrl)) return;
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Uri = WebPlayerUrl,
                    Text = EpisodeTitle,
                    Title = "Chia sẻ tập phim"
                });
            });

            PostCommentCommand = new Command(async () =>
            {
                if (!CanPost) return;
                try
                {
                    IsPosting = true; OnPropertyChanged(nameof(IsPosting));


                    var req = new CreateCommentRequest
                    {
                        userId = 1,
                        slug = MovieSlug,
                        displayName = "Phúc",
                        content = NewCommentText?.Trim(),
                        timeAgo = "vừa xong"
                    };
                    await _service.PostCommentAsync(req);
                    Comments.Insert(0, new CommentItem
                    {
                        DisplayName = "Phúc",
                        Content = NewCommentText?.Trim(),
                        TimeAgo = "vừa xong"
                    });
                    NewCommentText = string.Empty;
                    OnPropertyChanged(nameof(NewCommentText));
                    OnPropertyChanged(nameof(CommentsCount));
                }
                finally
                {
                    IsPosting = false; OnPropertyChanged(nameof(IsPosting));
                }
            });
        }

        public async Task LoadCommentsAsync()
        {
            if (string.IsNullOrWhiteSpace(MovieSlug)) return;

            IsLoading = true;
            var list = await _service.GetCommentsAsync(MovieSlug);
            Comments.Clear();
            foreach (var c in list)
                Comments.Add(c);
            IsLoading = false;
        }

        public async Task LoadFavoriteStatusAsync()
        {
            if (string.IsNullOrWhiteSpace(MovieSlug)) return;
            var list = await _service.GetFavoritesBySlugAsync(MovieSlug);

            LikesCount = list.Count; // đếm tổng lượt thích

            // Kiểm tra user hiện tại có trong list không
            IsLiked = list.Any(f => f.userId == CurrentUserId);


        }

        public async Task ToggleFavoriteAsync()
        {
            if (string.IsNullOrWhiteSpace(MovieSlug)) return;

            if (IsLiked)
            {
                // bỏ yêu thích
                var ok = await _service.RemoveFavoriteAsync(CurrentUserId, MovieSlug);
                if (ok)
                {
                    IsLiked = false;
                    LikesCount--;
                }
            }
            else
            {
                // thêm yêu thích
                var record = new FavoriteRecord { userId = CurrentUserId, slug = MovieSlug };
                var ok = await _service.AddFavoriteAsync(record);
                if (ok)
                {
                    IsLiked = true;
                    LikesCount++;
                }
            }
        }

        // --- Load từ link_embed ---
        public async Task LoadFromEmbedAsync(string? episodeName, string? linkEmbed,string? movieSlug)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            EpisodeTitle = episodeName;
            MovieSlug = movieSlug;

            try
            {
                if (string.IsNullOrWhiteSpace(linkEmbed))
                {
                    ErrorMessage = "Không có URL nhúng hợp lệ.";
                    return;
                }

                WebPlayerUrl = linkEmbed; // chỉ dùng link_embed
                await LoadCommentsAsync();
                await LoadFavoriteStatusAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi phát phim: " + ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

  
}
