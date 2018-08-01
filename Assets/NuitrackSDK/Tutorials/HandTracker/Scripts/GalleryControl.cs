using UnityEngine;
using UnityEngine.UI;

public class GalleryControl : MonoBehaviour
{
    enum ViewMode { Preview, View };
    ViewMode currentViewMode = ViewMode.Preview;

    [Header("Visualization")]

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] Sprite[] spriteCollection;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject imageItemPrefab;

    [SerializeField] CanvasGroup canvasGroup;

    [Header("Grid")]

    [Range(1, 10)]
    [SerializeField] int rowsNumber = 3;
    [Range(1, 10)]
    [SerializeField] int colsNumber = 4;

    Vector2 pageSize;
    int numberOfPages = 0;

    Vector2 defaultSize;   

    [Header("Scroll")]

    [Range(0.1f, 10)]
    [SerializeField] float scrollSpeed = 4f;

    float scrollStep = 0;

    [Header("View")]
    [SerializeField] RectTransform viewRect;

    Vector2 defaultPosition;

    [Range(0.1f, 16f)]
    [SerializeField] float animationSpeed = 2;

    ImageItem selectedItem = null;

    bool animated = false;
    float t = 0;
 
    int currentPage = 0;

    void Start()
    {
        pageSize = new Vector2(Screen.width, Screen.height);
        defaultSize = new Vector2(Screen.width / colsNumber, Screen.height / rowsNumber);

        Vector2 halfAdd = new Vector2(defaultSize.x / 2, -defaultSize.y / 2);

        int imagesOnPage = rowsNumber * colsNumber;
        numberOfPages = (int)Mathf.Ceil((float)spriteCollection.Length / imagesOnPage);

        int imageIndex = 0;

        for (int p = 0; p < numberOfPages; p++)
        {
            int imagesOnCurrentPage = Mathf.Min(spriteCollection.Length - p * imagesOnPage, imagesOnPage);

            for (int i = 0; i < imagesOnCurrentPage; i++)
            {
                GameObject currentItem = Instantiate(imageItemPrefab);
                currentItem.transform.SetParent(content.transform, false);

                RectTransform currentRect = currentItem.GetComponent<RectTransform>();
                currentRect.sizeDelta = defaultSize;

                float X = pageSize.x * p + defaultSize.x * (i % colsNumber);
                float Y = defaultSize.y * (i / colsNumber);

                currentRect.anchoredPosition = new Vector2(X, -Y) + halfAdd;

                Image currentImage = currentItem.GetComponent<Image>();
                currentImage.sprite = spriteCollection[imageIndex];
                imageIndex++;

                ImageItem currentImageItem = currentItem.GetComponent<ImageItem>();
                currentImageItem.OnClick += CurrentImageItem_OnClick;
            }
        }

        content.sizeDelta = new Vector2(Screen.width * numberOfPages, Screen.height);

        if (numberOfPages > 1)
            scrollStep = 1f / (numberOfPages - 1);

        NuitrackManager.onNewGesture += NuitrackManager_onNewGesture;        
    }

    private void OnDestroy()
    {
        NuitrackManager.onNewGesture -= NuitrackManager_onNewGesture;
    }

    private void CurrentImageItem_OnClick(ImageItem currentItem)
    {
        if (currentViewMode == ViewMode.Preview)
        {
            t = 0;
            currentViewMode = ViewMode.View;
            selectedItem = currentItem;

            canvasGroup.interactable = false;
            selectedItem.interactable = false;

            selectedItem.transform.SetParent(viewRect, true);
            defaultPosition = selectedItem.transform.localPosition;
        }
    }

    private void Update()
    {
        switch (currentViewMode)
        {
            case ViewMode.View:

                if (t < 1)
                {
                    t += Time.deltaTime * animationSpeed;

                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, t);

                    selectedItem.image.rectTransform.sizeDelta = Vector2.Lerp(selectedItem.image.rectTransform.sizeDelta, pageSize, t);
                    selectedItem.transform.localPosition = Vector2.Lerp(selectedItem.transform.localPosition, Vector2.zero, t);
                }

                break;

            case ViewMode.Preview:

                if (animated)
                {
                    if (t > 0)
                    {
                        t -= Time.deltaTime * animationSpeed;

                        canvasGroup.alpha = Mathf.Lerp(1, canvasGroup.alpha, t);

                        selectedItem.image.rectTransform.sizeDelta = Vector2.Lerp(defaultSize, selectedItem.image.rectTransform.sizeDelta, t);

                        selectedItem.transform.localPosition = Vector2.Lerp(defaultPosition, selectedItem.transform.localPosition, t);
                        selectedItem.transform.localRotation = Quaternion.Lerp(Quaternion.identity, selectedItem.transform.localRotation, t);
                        selectedItem.transform.localScale = Vector3.Lerp(Vector3.one, selectedItem.transform.localScale, t);
                    }
                    else
                    {
                        selectedItem.transform.SetParent(content, true);
                        selectedItem.interactable = true;
                        canvasGroup.interactable = true;
                        selectedItem = null;
                        animated = false;
                    }
                }
                else
                    scrollRect.horizontalScrollbar.value = Mathf.Lerp(scrollRect.horizontalScrollbar.value, scrollStep * currentPage, Time.deltaTime * scrollSpeed);

                break;
        }
    }

    private void NuitrackManager_onNewGesture(nuitrack.Gesture gesture)
    {
        switch (currentViewMode)
        {
            case ViewMode.Preview:

                if (gesture.Type == nuitrack.GestureType.GestureSwipeLeft)
                    currentPage = Mathf.Clamp(++currentPage, 0, numberOfPages);

                if (gesture.Type == nuitrack.GestureType.GestureSwipeRight)
                    currentPage = Mathf.Clamp(--currentPage, 0, numberOfPages);

                break;

            case ViewMode.View:

                if (gesture.Type == nuitrack.GestureType.GestureSwipeUp)
                {
                    currentViewMode = ViewMode.Preview;
                    animated = true;
                }
                break;
        }
    }
}
