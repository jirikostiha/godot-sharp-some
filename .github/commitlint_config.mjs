export default {
    // Dependabot autogenerates "<type>: Bump <dep> from x to y" with a capital "Bump";
    // its casing is not configurable, so skip those commits. Human commits use a
    // lowercase "bump" (per the commit-message rules) and stay fully linted.
    ignores: [(message) => /^[a-z]+(\([^)]+\))?!?: Bump /.test(message)],
    rules: {
        "body-leading-blank": [1, "always"],
        "body-max-line-length": [2, "always", 160],
        "footer-leading-blank": [1, "always"],
        "footer-max-line-length": [2, "always", 160],
        "header-max-length": [2, "always", 100],
        "scope-case": [2, "always", "lower-case"],
        "subject-case": [
            2,
            "never",
            ["upper-case", "pascal-case", "sentence-case", "start-case"],
        ],
        "subject-empty": [2, "never"],
        "subject-full-stop": [2, "never", "."],
        "type-case": [2, "always", "lower-case"],
        "type-empty": [2, "never"],
        "type-enum": [
            2,
            "always",
            [
                "build",
                "ci",
                "docs",
                "feat",
                "fix",
                "perf",
                "refactor",
                "revert",
                "test",
                "chore",
                "style",
            ],
        ],
    },
};
