/// <summary>
/// AudioClip 리소스 리스트가 자동으로 생성되는 클래스.
/// AudioClip 접근시 string으로 바로 접근하는것이 아니라 class.clipname 과 같은 형식으로 편하게 접근하기 위함.
/// AudioClipData.cs 파일의 CreateAudioClipList()함수에 의해 자동 생성되는 파일 및 클래스
/// </summary>

public static class AudioClipList {
	public static string Explosion = "Explosion";
	public static string GunReload = "GunReload";
	public static string GunShot = "GunShot";
	public static string Spring = "Spring";
	public static string SpringBack = "SpringBack";
}
