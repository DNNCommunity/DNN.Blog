import {
  BlockquotePlugin,
  BoldPlugin,
  CodePlugin,
  H1Plugin,
  H2Plugin,
  H3Plugin,
  HorizontalRulePlugin,
  ItalicPlugin,
  StrikethroughPlugin,
  UnderlinePlugin,
} from "@platejs/basic-nodes/react";
import { LinkPlugin } from "@platejs/link/react";
import { ImagePlugin, PlaceholderPlugin } from "@platejs/media/react";
import { KEYS } from "platejs";
import { ParagraphPlugin } from "platejs/react";

import {
  BlockquoteElement,
  BoldLeaf,
  CodeLeaf,
  H1Element,
  H2Element,
  H3Element,
  HrElement,
  ImageElement,
  ItalicLeaf,
  LinkElement,
  ParagraphElement,
  StrikethroughLeaf,
  UnderlineLeaf,
} from "./nodes";

export const editorPlugins = [
  ParagraphPlugin.withComponent(ParagraphElement),
  H1Plugin.withComponent(H1Element),
  H2Plugin.withComponent(H2Element),
  H3Plugin.withComponent(H3Element),
  BlockquotePlugin.withComponent(BlockquoteElement),
  HorizontalRulePlugin.withComponent(HrElement),
  BoldPlugin.withComponent(BoldLeaf),
  ItalicPlugin.withComponent(ItalicLeaf),
  UnderlinePlugin.withComponent(UnderlineLeaf),
  StrikethroughPlugin.withComponent(StrikethroughLeaf),
  CodePlugin.withComponent(CodeLeaf),
  LinkPlugin.configure({
    render: { node: LinkElement },
  }),
  ImagePlugin.configure({
    options: { disableUploadInsert: true },
    render: { node: ImageElement },
  }),
  PlaceholderPlugin.configure({
    options: {
      disableEmptyPlaceholder: true,
      disableFileDrop: true,
      uploadConfig: {
        image: {
          maxFileCount: 5,
          maxFileSize: "8MB",
          mediaType: KEYS.img,
          minFileCount: 1,
        },
      },
    },
  }),
];
