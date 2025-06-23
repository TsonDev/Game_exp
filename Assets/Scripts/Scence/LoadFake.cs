using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadFake : MonoBehaviour
{
    public Image loadingBarImage;        // Gán hình ảnh dạng Filled
    public float fakeDuration = 3f;      // thời gian chạy thanh giả lập
 

    void Start()
    {
        StartCoroutine(FakeLoad());
    }

    IEnumerator FakeLoad()
    {
        float time = 0f;

        while (time < fakeDuration)
        {
            time += Time.deltaTime;
            float progress = Mathf.Clamp01(time / fakeDuration);
            loadingBarImage.fillAmount = progress;
            yield return null;
        }
        loadingBarImage.gameObject.SetActive(false);

      
    }
    public void StartFakeLoading()
    {
        StartCoroutine(FakeLoad());
    }

}
