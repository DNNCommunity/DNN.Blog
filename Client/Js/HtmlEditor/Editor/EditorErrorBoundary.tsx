import React from "react";

type Props = { children: React.ReactNode };
type State = { error: Error | null };

export class EditorErrorBoundary extends React.Component<Props, State> {
  state: State = { error: null };

  static getDerivedStateFromError(error: Error): State {
    return { error };
  }

  componentDidCatch(error: Error, info: React.ErrorInfo) {
    console.error("HtmlEditor crashed", error, info.componentStack);
  }

  render() {
    if (this.state.error) {
      return (
        <div
          className="blog-he"
          style={{
            padding: 16,
            border: "1px solid #fca5a5",
            background: "#fef2f2",
            color: "#991b1b",
            whiteSpace: "pre-wrap",
            fontFamily: "ui-monospace, monospace",
            fontSize: 12,
          }}
        >
          <strong>HTML editor failed to load</strong>
          {"\n\n"}
          {this.state.error.message}
          {"\n\n"}
          {this.state.error.stack}
        </div>
      );
    }
    return this.props.children;
  }
}
