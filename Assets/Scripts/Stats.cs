public static class PlayerStats
{
    private static int _bulletDamage = 10;
    private static float _bulletSpeed = 5f;
    private static float _bulletSize = 1f;
    private static float _fireRate = 1f;
    private static float _reloadTime = 2f;

    private static int _health = 100;
    private static float _speed = 5f;
    private static float _jumpHeight = 2f;

    private static int _bounceCount = 0;

    // Get
    public static int GetBulletDamage() => _bulletDamage;
    public static float GetBulletSpeed() => _bulletSpeed;
    public static float GetBulletSize() => _bulletSize;
    public static float GetFireRate() => _fireRate;
    public static float GetReloadTime() => _reloadTime;

    public static int GetHealth() => _health;
    public static float GetSpeed() => _speed;
    public static float GetJumpHeight() => _jumpHeight;

    public static int GetBounceCount() => _bounceCount;


    // Increase
    public static void AddBulletDamage(float percentage) => _bulletDamage = (int)(_bulletDamage * (1 + percentage / 100));
    public static void AddBulletSpeed(float percentage) => _bulletSpeed *= (1 + percentage / 100);
    public static void AddBulletSize(float percentage) => _bulletSize *= (1 + percentage / 100);
    public static void AddFireRate(float percentage) => _fireRate *= (1 + percentage / 100);
    public static void AddReloadTime(float percentage) => _reloadTime *= (1 + percentage / 100);

    public static void AddHealth(float percentage) => _health = (int)(_health * (1 + percentage / 100));
    public static void AddSpeed(float percentage) => _speed *= (1 + percentage / 100);
    public static void AddJumpHeight(float percentage) => _jumpHeight *= (1 + percentage / 100);

    public static void IncreaseBounceCount(int increaseAmount) => _bounceCount += increaseAmount;

    // Decrease

    public static void DecreaseBulletDamage(float percentage) => _bulletDamage = (int)(_bulletDamage * (1 - percentage / 100));
    public static void DecreaseBulletSpeed(float percentage) => _bulletSpeed *= (1 - percentage / 100);
    public static void DecreaseBulletSize(float percentage) => _bulletSize *= (1 - percentage / 100);
    public static void DecreaseFireRate(float percentage) => _fireRate *= (1 - percentage / 100);
    public static void DecreaseReloadTime(float percentage) => _reloadTime *= (1 - percentage / 100);

    public static void DecreaseHealth(float percentage) => _health = (int)(_health * (1 - percentage / 100));
    public static void DecreaseSpeed(float percentage) => _speed *= (1 - percentage / 100);
    public static void DecreaseJumpHeight(float percentage) => _jumpHeight *= (1 - percentage / 100);

    public static void DecreaseBounceCount(int decreaseAmount) => _bounceCount -= decreaseAmount;
}
