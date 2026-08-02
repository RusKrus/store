namespace Store.Application.Interfaces.CartCookiesService;

public interface ICartCookiesService
{
    void SaveCartInCookies(Guid cartGuid);
    Guid? GetCartGuidFromCookies();
    public void DeleteCartFromCookies();
}