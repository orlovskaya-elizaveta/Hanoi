using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Algorithm : MonoBehaviour {

    int countOfDisks;

    Transform startStisk;
    GameObject disk;
    Button button;
    Text inputField;
    GameObject donePanel;

    int start;
    int temp;
    int end;

    //очередь для записи и дальнейшей визуализации шагов
    Queue<Step> steps;

    void Start () {

        //данные значения будут соответствовать порядковому номеру дочернего объекта нужного стика
        start = 0;
        temp = 1;
        end = 2;
        steps = new Queue<Step>();

        //подгружаем префаб диска
        disk = Resources.Load<GameObject>("disk");

        //находим среди дочерних объектов поле ввода
        inputField = transform.Find("InputField").GetChild(1).GetComponent<UnityEngine.UI.Text>();

        //находим кнопку и назначаем функцию на ее нажатие
        button = transform.Find("Button").GetComponent<Button>();
        button.onClick.AddListener(StartAlgorithm);

        //находим завершающую панель и делаем неактивной
        donePanel = transform.Find("DonePanel").gameObject;
        donePanel.SetActive(false);
    }

    void StartAlgorithm()
    {
        if (!System.String.IsNullOrEmpty(inputField.text))
        {
            //делаем кнопку и поле ввода неактивными
            button.interactable = false;
            inputField.GetComponentInParent<InputField>().interactable = false;

            //берем значение количества дисков из поля ввода
            int.TryParse(inputField.text, out countOfDisks);

            //задаем начальное состояние - заполняем дисками первый стик
            startStisk = transform.Find("startStisk").GetChild(0).transform;
            for (int i = countOfDisks; i > 0; i--)
            {
                //создаем новый диск и устанавливаем его дочерним к первому стику
                GameObject tmp = Instantiate(disk);
                tmp.transform.SetParent(startStisk);

                //каждый следующий диск делается меньше предыдущего на определенный шаг так, чтобы самый маленький диск был шириной 0.25, а самый большой - 1
                float width = 1 - i * 0.75f / countOfDisks;
                tmp.transform.localScale = new Vector3(width, tmp.transform.localScale.y, 0);
            }

            //запускаем, собственно, алгоритм
            MoveDisk(start, temp, end, countOfDisks);

            //запускаем визуализацию
            StartCoroutine(_draw(1));
        }
    }

    void MoveDisk(int start, int temp, int end, int countOfDisks)
    {
        //сам алгоритм решения задачи
        if (countOfDisks > 1)
            MoveDisk(start, end, temp, countOfDisks - 1);

        //записываем шаги для дальнейшей визуализации
        Step currStep = new Step(start, end);
        steps.Enqueue(currStep);

        if (countOfDisks > 1)
            MoveDisk(temp, start, end, countOfDisks - 1);
    }

    void DrawMove(Step step)
    {
        //находим начальный и конечный стики
        startStisk = transform.GetChild(step.from).GetChild(0);
        Transform endStisk = transform.GetChild(step.to).GetChild(0);

        //находим нужный диск
        Transform disk = startStisk.GetChild(0);

        //и устанавливаем его дочерним к стику, на который нужно его переложить
        disk.SetParent(endStisk);
        //чтобы новый с списке диск отображался выше остальных, вручную ставим его первым в списке дочерних объектов
        disk.SetAsFirstSibling();
    }

    IEnumerator _draw(float time)
    {
        while (steps.Count > 0)
        {
            //отрисовываем шаги с паузами в time секунд
            yield return new WaitForSeconds(time);
            DrawMove(steps.Dequeue());
        }
        //активируем панель с надписью "Решено"
        donePanel.SetActive(true);
    }

    struct Step
    {
        public int from;
        public int to;
        public Step(int start, int end)
        {
            from = start;
            to = end;
        }
    }
}