import * as React from 'react';

import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';

type HoverCardContextValue = {
  closeDelay: number;
  openDelay: number;
  setOpen: (open: boolean) => void;
};

const HoverCardContext = React.createContext<HoverCardContextValue>({
  closeDelay: 100,
  openDelay: 150,
  setOpen: () => {},
});

export function HoverCard({
  children,
  closeDelay = 100,
  onOpenChange,
  open,
  openDelay = 150,
}: {
  children: React.ReactNode;
  closeDelay?: number;
  onOpenChange?: (open: boolean) => void;
  open?: boolean;
  openDelay?: number;
}) {
  const setOpen = React.useCallback(
    (next: boolean) => {
      onOpenChange?.(next);
    },
    [onOpenChange]
  );

  return (
    <HoverCardContext.Provider value={{ closeDelay, openDelay, setOpen }}>
      <Popover onOpenChange={onOpenChange} open={open}>
        {children}
      </Popover>
    </HoverCardContext.Provider>
  );
}

export function HoverCardTrigger({
  onMouseEnter,
  onMouseLeave,
  ...props
}: React.ComponentProps<typeof PopoverTrigger>) {
  const { closeDelay, openDelay, setOpen } = React.useContext(HoverCardContext);
  const openTimer = React.useRef<number | undefined>(undefined);
  const closeTimer = React.useRef<number | undefined>(undefined);

  const clearTimers = () => {
    window.clearTimeout(openTimer.current);
    window.clearTimeout(closeTimer.current);
  };

  return (
    <PopoverTrigger
      {...props}
      onMouseEnter={(event) => {
        onMouseEnter?.(event);
        clearTimers();
        openTimer.current = window.setTimeout(() => setOpen(true), openDelay);
      }}
      onMouseLeave={(event) => {
        onMouseLeave?.(event);
        clearTimers();
        closeTimer.current = window.setTimeout(() => setOpen(false), closeDelay);
      }}
    />
  );
}

export function HoverCardContent({
  onMouseEnter,
  onMouseLeave,
  ...props
}: React.ComponentProps<typeof PopoverContent>) {
  const { closeDelay, setOpen } = React.useContext(HoverCardContext);
  const closeTimer = React.useRef<number | undefined>(undefined);

  return (
    <PopoverContent
      {...props}
      onMouseEnter={(event) => {
        onMouseEnter?.(event);
        window.clearTimeout(closeTimer.current);
        setOpen(true);
      }}
      onMouseLeave={(event) => {
        onMouseLeave?.(event);
        closeTimer.current = window.setTimeout(() => setOpen(false), closeDelay);
      }}
    />
  );
}
