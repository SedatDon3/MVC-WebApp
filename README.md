# Описание
### программного кода модулей "Расписание" и "Сетка"<br />ИС "Электронный журнал преподавателя"

В рамках выпускной квалификационной работы была разработана (частично) web-ориентированная информационная система "Электронный журнал преподавателя" для Ярославского Государственного Технического Университета. Система предназначена для объединения учета посещаемости и контроля успеваемости студентов ЯГТУ, а также расписания преподавателя воедино. 

## Разработка

Разработка системы проходила в соответствии с паттерном MVC с использованием ASP.NET на языке C#. В качестве СУБД была выбрана MongoDB в связи с тем, что данные, которые необходимо хранить, являются сложно структурированными, иерархическими и имеют относительную динамику, реализовать которую с помощью реляционных СУБД непросто.
Для реализации представлений или пользовательских интерфейсов были использованы следующие синтаксис, языки и фреймворки:
- Razor, HTML5, CSS3, Bootstrap
- JavaScript, jQuery

При разработке было решено поделить систему на пять функциональных модулей:
- учебный план направления,
- рабочая программа преподавателя,
- пользователи,
- расписание преподавателя,
- сетка

## О модулях

Модули "Расписание" и "Сетка" тесно связаны друг с другом, потому что, так называемая, сетка составляется на основе расписания преподавателя. Поэтому было решено не выделять отдельного контроллера для модуля "Сетка". Однако модули не были до конча разработаны, в частности "Сетка".
Далее представлено краткое описание файлов, относящихся к одной из составлющих шаблона MVC.

### Model

ScheduleView.cs
SubgroupView.cs
Grid.cs
ScheduleContext.cs

### Controller

ScheduleController.cs

### View

**Index.cshtml** - представление, отображающее расписание и являющееся домашней страницей для пользователей с ролью преподавателя.<br />
<img src="https://github.com/SedatDon3/MVC-WebApp/blob/master/Screenshots/Index.png?raw=true" width="656" height="462">

\_SetGrid.schtml
Grid.cshtml


![](https://github.com/SedatDon3/MVC-WebApp/blob/master/Screenshots/Grid.png?raw=true)
