# SPDX-FileCopyrightText: 2022 EmoGarbage404 <98561806+EmoGarbage404@users.noreply.github.com>
# SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
# SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
# SPDX-FileCopyrightText: 2023 Tom Leys <tom@crump-leys.com>
# SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
# SPDX-FileCopyrightText: 2024 psykana <36602558+psykana@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 Aiden <aiden@djkraz.com>
#
# SPDX-License-Identifier: AGPL-3.0-or-later

zombie-transform = {CAPITALIZE(THE($target))} turned into a zombie!
zombie-infection-greeting = You have become a zombie. Your goal is to seek out the living and to try to infect them.  Work together with the other zombies and remaining initial infected to overtake the station.
zombie-infection-stage-pale = Your head spins.
zombie-infection-stage-slowed = Your body grows heavier.
zombie-infection-stage-final = A hunger awakens within you.
zombie-infection-looks-greenish = [color=darkseagreen]{CAPITALIZE(SUBJECT($target))} {CONJUGATE-BASIC($target, "look", "looks")}... greenish?[/color]

zombie-generic = zombie
zombie-name-prefix = zombified {$baseName}
zombie-role-desc =  A malevolent creature of the dead.
zombie-role-rules = You are a [color={role-type-team-antagonist-color}][bold]{role-type-team-antagonist-name}[/bold][/color]. Search out the living and bite them in order to infect them and turn them into zombies. Work together with the other zombies and remaining initial infected to overtake the station.

zombie-permadeath = This time, you're dead for real.

zombification-resistance-coefficient-value = - [color=violet]Infection[/color] chance reduced by [color=lightblue]{$value}%[/color].

# Goob
zombie-cured-popup = The zombie infection vanishes without a trace!
zombie-cure-failed-popup = The cure fails to take hold!

zombie-role-menu-title = You have become a zombie
zombie-role-menu-summary = You are part of the outbreak. Hunt the living, infect them with bites, and work together with other zombies.
zombie-role-menu-communication-title = Communication
zombie-role-menu-communication-1 = Speak in short, simple phrases, like "brains there with boomstick". Other zombies can sometimes understand you; the living only hear groans.
zombie-role-menu-communication-2 = Do not use normal speech for detailed plans, negotiations, or regular conversation. Spelling mistakes are allowed.
zombie-role-menu-conditions-title = Conditions
zombie-role-menu-conditions-1 = In normal speech, messages longer than 14 characters turn into mumbling. Exceptions: the word "brains" and spaces.
zombie-role-menu-conditions-2 = You can only say something relatively smart with a 10-second cooldown.
zombie-role-menu-confirm = Got it
