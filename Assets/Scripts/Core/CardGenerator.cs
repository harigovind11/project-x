using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardGenerator : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject cardPrefab;

    [Header("Layout Settings")]
    [SerializeField, Tooltip("The visual space between cards in world units.")]
    private float cardSpacing = 0.2f;
    [SerializeField, Tooltip("Padding from the screen edges in world units.")]
    private float screenPadding = 0.5f;

    // Generates the full board of cards based on a LevelData asset.
    public void GenerateBoard(LevelData levelData)
    {
        // Clear any existing cards before generating a new board
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        List<CardData> cardsToCreate = new List<CardData>();
        foreach (var cardData in levelData.cardsToUse)
        {
            cardsToCreate.Add(cardData);
            cardsToCreate.Add(cardData);
        }

        cardsToCreate = cardsToCreate.OrderBy(x => Random.value).ToList();

        InstantiateAndScaleGrid(levelData.columns, levelData.rows, cardsToCreate);
    }

    private void InstantiateAndScaleGrid(int columns, int rows, List<CardData> cards)
    {
        SpriteRenderer prefabRenderer = cardPrefab.GetComponent<SpriteRenderer>();
        if (prefabRenderer == null || prefabRenderer.sprite == null)
        {
            Debug.LogError("Card Prefab needs a SpriteRenderer with a default sprite assigned to calculate its size!");
            return;
        }
        float cardWidth = prefabRenderer.sprite.bounds.size.x;
        float cardHeight = prefabRenderer.sprite.bounds.size.y;

        // Get the dimensions of the camera's view in world units.
        Camera mainCamera = Camera.main;
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Define a "safe area" by subtracting the padding from the total screen dimensions.
        float safeScreenWidth = screenWidth - (2f * screenPadding);
        float safeScreenHeight = screenHeight - (2f * screenPadding);

        // 1. Calculate the total space the gaps will occupy (this space will not be scaled).
        float totalSpacingWidth = (columns > 1) ? (columns - 1) * cardSpacing : 0;
        float totalSpacingHeight = (rows > 1) ? (rows - 1) * cardSpacing : 0;

        // 2. Calculate the screen space remaining for the cards themselves.
        float availableWidthForCards = safeScreenWidth - totalSpacingWidth;
        float availableHeightForCards = safeScreenHeight - totalSpacingHeight;

        // 3. Calculate the scale factor based on the space available for cards.
        float totalCardWidthUnscaled = columns * cardWidth;
        float totalCardHeightUnscaled = rows * cardHeight;

        float scaleFactorX = availableWidthForCards / totalCardWidthUnscaled;
        float scaleFactorY = availableHeightForCards / totalCardHeightUnscaled;
        float finalScale = Mathf.Min(scaleFactorX, scaleFactorY);

        // Safety check: If spacing/padding is too large, scale can become zero or negative.
        if (finalScale <= 0)
        {
            Debug.LogWarning("Padding and/or spacing is too large for the grid and screen size. Grid may not display correctly.");
            finalScale = 0.01f; // Clamp to a small positive value
        }

        // 4. Apply the final scale to the parent transform.
        transform.localScale = new Vector3(finalScale, finalScale, 1f);

        // 5. Calculate positions in LOCAL space. To achieve a final world space gap of `cardSpacing`,
        // the local space gap must be "un-scaled" (i.e., divided by the scale factor).
        float localCardWidth = cardWidth;
        float localCardHeight = cardHeight;
        float localSpacingX = cardSpacing / finalScale;
        float localSpacingY = cardSpacing / finalScale;

        // 6. Calculate total grid dimensions and start position in LOCAL space.
        float localGridWidth = (columns * localCardWidth) + ((columns > 1) ? (columns - 1) * localSpacingX : 0);
        float localGridHeight = (rows * localCardHeight) + ((rows > 1) ? (rows - 1) * localSpacingY : 0);

        Vector2 startPos = new Vector2(
            -localGridWidth / 2f + localCardWidth / 2f,
            localGridHeight / 2f - localCardHeight / 2f
        );

        int cardIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (cardIndex < cards.Count)
                {
                    Vector2 localPos = new Vector2(
                        startPos.x + col * (localCardWidth + localSpacingX),
                        startPos.y - row * (localCardHeight + localSpacingY)
                    );

                    GameObject cardObj = Instantiate(cardPrefab, this.transform);
                    cardObj.transform.localPosition = localPos;
                    cardObj.name = $"Card_{row}_{col}";

                    cardObj.GetComponent<CardView>().Setup(cards[cardIndex]);
                    cardIndex++;
                }
            }
        }
    }
}