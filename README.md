# CyberiadaLogic
Данный пакет состоит из 4 отдельных модулей:
* **CyberiadaGraph** — модуль, содержащий структуру графа и всех его элементов.
* **Converter** — модуль, предназначенный для конвертации объектов ```CyberiadaGraphDocument``` в документы формата GraphML.
* **LogicBridge** — модуль, обеспечивающий взаимодействие между графами и интерпретатором, который осуществляет исполнение команд, хранимых в графах.
* **EditorCore** — модуль, использующийся как основа для редактирования CyberiadaGraph, а также его визуального представления.

### CyberiadaGraph
В модуле **CyberiadaGraph** описывается структура графа и его элементы.  
Граф состоит из множества узлов и ребер.  
Логика узлов хранится в классах ```Node``` и ```NodeData```, а логика ребер — в классах ```Edge``` и ```EdgeData```.  
Узлы соединены между собой ребрами.  
Узлы могут содержать внутри себя дочерние узлы, таким образом, в **CyberiadaGraph** существует поддержка подграфов.  
Данные, хранимые в узлах и ребрах, тесно связаны с документацией CyberiadaGraphML и интерпретатором, который будет выполнять описываемое данным графом поведение.  
Также внутри элементов графа могут содержаться данные, которые отвечают за визуальное представление графа, например расположение элементов и их названия.

### Converter
Данный модуль отвечает за конвертацию объектов ```CyberiadaGraphDocument``` в файлы в формате GraphML и наоборот.  
**CyberiadaGraphDocument** состоит из корневого графа и различных метаданных, специфичных для конкретной целевой платформы, на которой будет исполняться данный граф.  
Конвертация происходит с использованием библиотеки ```System.Xml.Linq```.

### LogicBridge
В данном модуле находится набор интерфейсов, которые позволяют определить набор правил для обработки строковых команд интерпретатором.
* ```ICommandBus``` используется для обработки команд.
* ```IVariableBus``` — для переменных.
* ```IEventBus``` — для событий.

### EditorCore
Данный модуль используется для создания пользовательского редактора графов.  
Основной класс данного модуля — ```GraphEditor```, который имеет набор методов для добавления и удаления различных элементов графа, а также перестроения визуального состояния графа.  
```GraphEditor``` изменяет структуру графа и использует интерфейс ```IGraphElementViewFactory``` для изменения визуального представления графа.  
В программах, использующих данный модуль, подразумевается, что пользователь должен реализовать данный интерфейс самостоятельно. Конечная реализация может сильно меняться в зависимости от выбранной графической библиотеки.

## Инструкции по интеграции редактора графов в сторонний проект
### 1. Реализация представлений элементов графа
Необходимо реализовать представления элементов графа:

| Название интерфейса   | Описание интерфейса                                                                                                |
|-----------------------|--------------------------------------------------------------------------------------------------------------------|
| ```IGraphView```      | Представление графа, использующееся для хранения других элементов, а также в качестве подграфа, вложенного в узел. |
| ```INodeView```       | Представление узла, позволяющее устанавливать родительское представление графа.                                    |
| ```INodeEventView```  | Представление состояния конкретного узла. Поддерживает редактирование события и условия перехода.                  |
| ```INodeActionView``` | Представление поведения конкретного узла. Поддерживает установку списка параметров, распознаваемых поведением.     |
| ```IEdgeView```       | Представление ребра, поддерживающее редактирование события и условия перехода.                                     |
| ```IEdgeActionView``` | Представление поведения конкретного ребра. Поддерживает установку списка параметров, распознаваемых поведением.    |

### 2. Реализация фабрики представлений элементов графа
Вся работа по изменению графа происходит с использованием класса ```GraphEditor```.
```GraphEditor``` хранит взаимно однозначное соответствие элементов графа и представлений.
Внутри себя ```GraphEditor``` использует клиентскую реализацию интерфейса ```IGraphElementViewFactory``` для управления жизненным циклом создаваемых представлений.
При вызове методов класса ```GraphEditor``` для создания/удаления представлений будет исполняться код, отвечающий за создание/удаление элементов исходного графа.
Для заполнения элементов представления полезной информацией ```GraphEditor``` использует клиентские реализации интерфейсов представлений.
Эти изменения также попадают в исходный граф.
В данном пункте необходимо реализовать интерфейс ```IGraphElementViewFactory```.
Список методов, реализуемых данным интерфейсом:

| Название интерфейса элемента графа                                                                                                            | Описание интерфейса                           |
|-----------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------|
| ```IGraphView CreateGraphView(string graphID, INodeView parentNodeView)```                                                                    | Создает представление графа.                  |
| ```NodeView CreateNodeView(NodeVisualData visualData, string vertex, bool layoutAutomatically)```                                             | Создает представление узла.                   |
| ```INodeEventView CreateNodeEventView(INodeView nodeView, string triggerID, Event @event)```                                                  | Создает представление состояния.              |
| ```IEdgeView CreateEdgeView(INodeView sourceNode, INodeView targetNode, EdgeVisualData edgeVisualData, string triggerID, string condition)``` | Создает представление поведения в cостоянии.  |
| ```IEdgeActionView CreateEdgeActionView(IEdgeView edgeView, string actionID)```                                                               | Создает представление ребра.                  |
| ```void DestroyElementView(IGraphElementView view)```                                                                                         | Создает представление поведения в переходе.   |

### 3. Создание источника контекста выполнения
Для того чтобы задать события, поведения и условия, которые будут поддерживаться конкретным редактируемым графом, необходимо создать источник контекста выполнения.  
Источник контекста выполнения — это класс, реализующий интерфейс ```IExecutionContextSource```.  
Данный интерфейс позволяет получить весь контекст в виде множества строк.
* ```IEnumerable<string> GetEvents()``` — возвращает перечисление событий, поддерживаемых интерпретатором.
В общем виде событие — это алфавитно-цифровая строка с подчёркиваниями, начинающаяся с буквенного символа. 
Также допустимо использование пробелов и точки для компонентно-ориентированных платформ.
* ```IEnumerable<ActionData> GetActions()``` — возвращает перечисление поведений, поддерживаемых интерпретатором.
Поведение состоит из списка параметров, которые могут быть подставлены интерпретатором для выполнения поведения. 
У каждого параметра есть имя, тип принимаемого значения и область определения, принимаемых параметром значений.
* ```IEnumerable<string> GetVariables()``` — возвращает все переменные, поддерживаемые интерпретатором.
Переменная представляет собой произвольную строку, которую обрабатывает интерпретатор.
Переменные используется для проверки условий на истинность.

### 4. Создание клиентского класса ```MyGraphEditor``` (Здесь будет пример реализации визуального редактора графов)

## © 2024 Кружковое движение  
