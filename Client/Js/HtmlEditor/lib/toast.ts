function warn(msg: string) {
  console.warn(msg);
}

export const toast = Object.assign(warn, {
  error: warn,
  success: warn,
});

export default toast;
