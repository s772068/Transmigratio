using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "New News", menuName = "ScriptableObjects/Create News", order = 2)]
    public class NewsSO : ScriptableObject
    {
        [Header("����� ���������")]
        [Tooltip("��������� � �������� ������������ ��� �������� �� ������� �����������")]
        [SerializeField] private string _titleName;
        [SerializeField] private Sprite _image;
        [SerializeField] private string _descriptionName;
        [Header("��� �������")]
        [SerializeField] private NewsType _type;
        [Tooltip("���� ������� - ���� �� ����� ��������")]
        [SerializeField] private int _dateInvoke = 0;

        public string Title => _titleName;
        public string Description => _descriptionName;
        public Sprite Image => _image;
        public NewsType Type => _type;
        public int DateInvoke => _dateInvoke;

        public enum NewsType
        {
            Date,
            Trigger
        }
    }
}
