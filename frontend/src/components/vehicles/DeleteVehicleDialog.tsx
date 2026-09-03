import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";

interface DeleteVehicleDialogProps {
  onConfirm: () => void;
  isPending: boolean;
}

export function DeleteVehicleDialog({
  onConfirm,
  isPending,
}: DeleteVehicleDialogProps) {
  return (
    <AlertDialog>
      <AlertDialogTrigger
        render={<Button variant="destructive" disabled={isPending} />}
      >
        Delete Vehicle
      </AlertDialogTrigger>

      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Delete Vehicle?</AlertDialogTitle>

          <AlertDialogDescription>
            Are you sure you want to delete this vehicle? This action cannot be
            undone.
          </AlertDialogDescription>
        </AlertDialogHeader>

        <AlertDialogFooter>
          <AlertDialogCancel>Cancel</AlertDialogCancel>

          <AlertDialogAction onClick={onConfirm}>Delete</AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
