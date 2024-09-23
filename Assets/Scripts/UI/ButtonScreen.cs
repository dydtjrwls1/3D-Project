using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonScreen : ScreenBase
{
    Button[] buttons;

    TextMeshProUGUI titleText;

    float saturation = 1f;
    float duration = 0.1f;

    protected override void Awake()
    {
        base.Awake();

        Transform child;

        buttons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            child = transform.GetChild(i);
            Button button = child.GetComponent<Button>();
            buttons[i] = button;

            int sceneNum = i + 1;
            button.onClick.AddListener(() => 
            {
                FadeManager.Instance.FadeAndLoadScene(sceneNum);
            });
        }

        child = transform.GetChild(3);
        titleText = child.GetComponent<TextMeshProUGUI>(); 
    }

    protected override void Start()
    {
        base.Start();

        Sequence sequence = DOTween.Sequence().SetLoops(-1, LoopType.Restart);

        for(float hue = 0.0f; hue < 1f; hue += 0.1f)
        {
            Color color = Color.HSVToRGB(hue, saturation, 1f);
            sequence.Append(titleText.DOColor(color, duration));
        }

        sequence.Play();
    }
}
