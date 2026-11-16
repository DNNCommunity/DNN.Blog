import type { SlateEditor } from 'platejs';

import {
  createSlatePlugin,
  createTextSubstitutionInputRule,
  KEYS,
} from 'platejs';

const isTextSubstitutionBlocked = (editor: SlateEditor) =>
  editor.api.some({
    match: {
      type: [editor.getType(KEYS.codeBlock)],
    },
  });

const createAutoformatTextSubstitutionRule = ({
  patterns,
}: {
  patterns: Parameters<typeof createTextSubstitutionInputRule>[0]['patterns'];
}) =>
  createTextSubstitutionInputRule({
    enabled: ({ editor }) => !isTextSubstitutionBlocked(editor),
    patterns,
  });

const arrowsRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '?', match: '->' },
    { format: '?', match: '<-' },
    { format: '?', match: '=>' },
    { format: '?', match: ['<=', '=='] },
  ],
});

const comparisonsRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '?', match: '!>' },
    { format: '?', match: '!<' },
    { format: '=', match: '>=' },
    { format: '=', match: '<=' },
    { format: '?', match: '!>=' },
    { format: '?', match: '!<=' },
  ],
});

const equalityRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '?', match: '!=' },
    { format: '=', match: '==' },
    { format: '?', match: ['!==', '?='] },
    { format: '˜', match: '~=' },
    { format: '?', match: '!~=' },
  ],
});

const fractionsRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '½', match: '1/2' },
    { format: '?', match: '1/3' },
    { format: '¼', match: '1/4' },
    { format: '?', match: '1/5' },
    { format: '?', match: '1/6' },
    { format: '?', match: '1/7' },
    { format: '?', match: '1/8' },
    { format: '?', match: '1/9' },
    { format: '?', match: '1/10' },
    { format: '?', match: '2/3' },
    { format: '?', match: '2/5' },
    { format: '¾', match: '3/4' },
    { format: '?', match: '3/5' },
    { format: '?', match: '3/8' },
    { format: '?', match: '4/5' },
    { format: '?', match: '5/6' },
    { format: '?', match: '5/8' },
    { format: '?', match: '7/8' },
  ],
});

const legalRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '™', match: ['(tm)', '(TM)'] },
    { format: '®', match: ['(r)', '(R)'] },
    { format: '©', match: ['(c)', '(C)'] },
  ],
});

const legalHtmlRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '™', match: '&trade;' },
    { format: '®', match: '&reg;' },
    { format: '©', match: '&copy;' },
    { format: '§', match: '&sect;' },
  ],
});

const operatorsRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '±', match: '+-' },
    { format: '‰', match: '%%' },
    { format: '?', match: ['%%%', '‰%'] },
  ],
});

const punctuationRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '»', match: '>>' },
    { format: '«', match: '<<' },
  ],
});

const smartQuotesRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: ['“', '”'], match: '"' },
    { format: ['‘', '’'], match: "'" },
  ],
});

const subscriptNumbersRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '0', match: '~0' },
    { format: '1', match: '~1' },
    { format: '2', match: '~2' },
    { format: '3', match: '~3' },
    { format: '4', match: '~4' },
    { format: '5', match: '~5' },
    { format: '6', match: '~6' },
    { format: '7', match: '~7' },
    { format: '8', match: '~8' },
    { format: '9', match: '~9' },
  ],
});

const subscriptSymbolsRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '?', match: '~+' },
    { format: '?', match: '~-' },
  ],
});

const superscriptNumbersRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '°', match: '^0' },
    { format: '¹', match: '^1' },
    { format: '²', match: '^2' },
    { format: '³', match: '^3' },
    { format: '4', match: '^4' },
    { format: '5', match: '^5' },
    { format: '6', match: '^6' },
    { format: '7', match: '^7' },
    { format: '8', match: '^8' },
    { format: '?', match: '^9' },
  ],
});

const superscriptSymbolsRule = createAutoformatTextSubstitutionRule({
  patterns: [
    { format: '°', match: '^o' },
    { format: '?', match: '^+' },
    { format: '?', match: '^-' },
  ],
});

const AutoformatShortcutsPlugin = createSlatePlugin({
  key: 'autoformatShortcuts',
  inputRules: [
    legalRule,
    legalHtmlRule,
    arrowsRule,
    comparisonsRule,
    equalityRule,
    fractionsRule,
    operatorsRule,
    punctuationRule,
    smartQuotesRule,
    subscriptNumbersRule,
    subscriptSymbolsRule,
    superscriptNumbersRule,
    superscriptSymbolsRule,
  ],
});

export const AutoformatKit = [AutoformatShortcutsPlugin];
