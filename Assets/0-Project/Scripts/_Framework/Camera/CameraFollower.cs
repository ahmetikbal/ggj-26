using UnityEngine;

public class CameraFollower : MonoBehaviour
{
	public static CameraFollower Instance;

	[SerializeField] public Transform player;

	//public float smoothTime = 0.5f;
	public float verticalSmoothTime = 0.1f;
	public float horizontalSmoothTime = 0.3f;

	private Vector3 velocity;

	private Vector3 offset;

    private void Awake()
    {
		if (Instance == null)
			Instance = this;
    }

    private void Start()
	{
		offset = transform.position - player.position;
	}

	private void FixedUpdate()
	{
		//transform.position = Vector3.SmoothDamp(transform.position, player.position + offset, ref velocity, smoothTime);

		transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.position.x + offset.x, transform.position.y, transform.position.z), ref velocity, horizontalSmoothTime);
		transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, transform.position.y, player.position.z + offset.z), ref velocity, verticalSmoothTime);
	}
}