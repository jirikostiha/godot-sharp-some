# Agent Instructions

<!-- ai-kit:begin - generated, do not edit inside this block -->
## Where to look

Find rules, skills, workflows, sub-agents and glossaries in this order. The first match of a
name wins; a lower source never overrides a higher one.

1. **This repository.** Its own `.ai/` layer (`rules/`, `skills/<name>/SKILL.md`, `projects/`)
   and any instruction file the repository itself keeps.
2. **The kit beside this repository.** `../ai-kit/`, in the same parent folder as this
   repository's root. Start with `../ai-kit/rules/global.rule.md`, then look in `rules/`,
   `skills/<name>/SKILL.md`, `workflows/`, `agents/` and `glossaries/`.
   If `../ai-kit/` does not exist, clone it:
   `git clone https://github.com/jirikostiha/ai-kit.git ../ai-kit` (private; uses this
   machine's git credentials). If the clone fails, say so and continue with step 3.
   Never edit the kit as part of a task in this repository.
3. **Everything else.** `.ai/kit/` (a generated mirror; it may lag behind), user-level skills
   (`~/.claude/skills/`, `~/.agents/skills/`), and only then general knowledge.
<!-- ai-kit:end -->
