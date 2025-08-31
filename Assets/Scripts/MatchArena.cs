using System;
using Core.Match;
using UnityEngine;

public class MatchArena : MonoBehaviour
{
    //SERIALIZED
    [SerializeField] private Transform prefabPiecesContainer;
    [SerializeField] private Transform gamePiecesContainer;
    
    //PRIVATES
    private Board board;

    public void CreateBoard()
    {
        var boardData = new BoardData();
        // instantiate all pieces from white and black containers to gameplay container
        var characterPrefabs = prefabPiecesContainer.GetComponentsInChildren<Character>(true);
        foreach (Character prefab in characterPrefabs)
        {
            var character = Instantiate(prefab, gamePiecesContainer);
            int positionX = Mathf.Clamp(Mathf.FloorToInt(character.transform.position.x), 0, 7);
            int positionY = Mathf.Clamp(Mathf.FloorToInt(character.transform.position.z), 0, 7);
            var index = Board.GetIndex(positionX, positionY);
            if (boardData.Characters[index] != null)
            {
                throw new Exception("Two pieces in the same position");
            }
            boardData.Characters[index] = character;
        }
        board = new Board(boardData);
    }

    public void DestroyBoard()
    {
        board.ClearBoard();
        foreach (Transform piece in gamePiecesContainer)
        {
            Destroy(piece.gameObject);
        }
    }
}