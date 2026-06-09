<div align="center"><img alt="Amour-EE logo" src="https://github.com/user-attachments/assets/7b448c5b-3b69-46be-a4b4-37db196b9a63" width="700px" /></div>

---


Amour - это русскоязычный форк [Goob Station](https://github.com/Goob-Station/Goob-Station).

Space Station 14 - это ремейк SS13, который работает на собственном движке  [Robust Toolbox](https://github.com/space-wizards/RobustToolbox), собственном игровом движке, написанном на C#.

Поскольку это хард-форк, любой код, взятый из другого апстрима, не может быть напрямую замержен сюда, а должен быть перенесен.

## Ссылки

[Discord](https://discord.gg/3cwTnsVyn6) | [Steam](https://store.steampowered.com/app/1255460/Space_Station_14/) | [Основной репозиторий](https://github.com/Simple-Station/Einstein-Engines)

## Сборка

Следуйте [гайду от Space Wizards](https://docs.spacestation14.com/en/general-development/setup/setting-up-a-development-environment.html) по настройке рабочей среды, но учитывайте, что наши репозитории отличаются и некоторые вещи могут отличаться.
Мы предлагаем несколько скриптов, показанных ниже, чтобы облегчить работу.

### Необходимые зависимости

> - Git
> - .NET SDK 9.0.101


### Windows

> 1. Склонируйте данный репозиторий
> 2. Запустите `git submodule update --init --recursive` в командной строке, чтобы скачать движок игры
> 3. Запускайте `Scripts/bat/buildAllDebug.bat` после любых изменений в коде проекта
> 4. Запустите `Scripts/bat/runQuickAll.bat`, чтобы запустить клиент и сервер
> 5. Подключитесь к локальному серверу и играйте

### Linux

> 1. Склонируйте данный репозиторий.
> 2. Запустите `git submodule update --init --recursive` в командной строке, чтобы скачать движок игры
> 3. Запускайте `Scripts/sh/buildAllDebug.sh` после любых изменений в коде проекта
> 4. Запустите `Scripts/sh/runQuickAll.sh`, чтобы запустить клиент и сервер
> 5. Подключитесь к локальному серверу и играйте

### MacOS

> Предположительно, также, как и на Линуксе.

## Лицензия

Содержимое, добавленное в этот репозиторий после коммита 87c70a89a67d0521a56388e6b1c3f2cb947943e4 (`17 February 2024 23:00:00 UTC`), распространяется по лицензии GNU Affero General Public License версии 3.0, если не указано иное.
См. [LICENSE-AGPLv3](./LICENSE-AGPLv3.txt).

Содержимое, добавленное в этот репозиторий до коммита 87c70a89a67d0521a56388e6b1c3f2cb947943e4 (`17 February 2024 23:00:00 UTC`) распространяется по лицензии MIT, если не указано иное.
См. [LICENSE-MIT](./LICENSE-MIT.txt).

Большинство ресурсов лицензировано под [CC-BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/), если не указано иное. Лицензия и авторские права на ресурсах указаны в файле метаданных.
[Example](./Resources/Textures/Objects/Tools/crowbar.rsi/meta.json).
